using Asp.Versioning;
using AspNetCoreRateLimit;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UrlShortener.Core.Security.Authentication;
using UrlShortener.Core.Security.Identity;
using UrlShortener.Core.Services.Implementations;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data.Repositories;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Validation;


namespace UrlShortener.Extensions
{
    public static class ServiceExtensions
    {
        

        public class LazyService<T> : Lazy<T> where T : class
        {
                public LazyService(IServiceProvider provider)
                    : base(() => provider.GetRequiredService<T>()) {}
        }

        public static void RemoveGlobalModelValidator(this IServiceCollection services) =>
            services.Replace(
                new ServiceDescriptor(typeof(IObjectModelValidator),
                typeof(EmptyModelValidator),
                ServiceLifetime.Singleton)
            );

        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
            });

        public static void ConfigureBusinessLayerServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(Lazy<>), typeof(LazyService<>));
            
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IPlanService, PlanService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IUrlRepository, UrlRepository>();
            services.AddScoped<IUrlService, UrlService>();

            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionService, PermissionService>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IShortCodeService, ShortCodeService>();

        }

        public static void ConfigureLoggerService(this IServiceCollection services)
            => services.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureUserContext(this IServiceCollection services)
            => services.AddScoped<IUserContextAccessor, UserContextAccessor>();

        public static void ConfigureRepositoryManager(this IServiceCollection services) 
            => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services)
            => services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureIISIntegration(this IServiceCollection services) 
            => services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
                options.AuthenticationDisplayName = "Windows Auth";
                options.ForwardClientCertificate = true;
            });

        public static void ConfigureVersioning(this IServiceCollection services)
            => services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(
                        new QueryStringApiVersionReader("api-version"),
                        new HeaderApiVersionReader("X-Version"),
                        new MediaTypeApiVersionReader("ver"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        public static void ConfigureResponseCaching(this IServiceCollection services)
            => services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) 
            => services.AddHttpCacheHeaders( (expirationOpt) => 
            { 
                expirationOpt.MaxAge = 65;
                expirationOpt.CacheLocation = CacheLocation.Private; 
            }, 
                (validationOpt) => validationOpt.MustRevalidate = true
            );

        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 30,
                    Period = "1m"
                }
            };

            services.Configure<IpRateLimitOptions>(opt => {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            ILoggerManager logger = services.BuildServiceProvider().GetRequiredService<ILoggerManager>();
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

            var SigningKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");
            
            if(jwtOptions is null)
                logger.LogCritical("JWT Option are missing !!!!");

            if (string.IsNullOrEmpty(SigningKey))
                logger.LogCritical("SigningKey is missing !!!!");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true; // if valid save the token string
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audiences,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(SigningKey)
                            ),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {

                            if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
                                return Task.CompletedTask;

                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.Headers["X-Auth-Error"] = "token_expired";
                                context.NoResult();
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
