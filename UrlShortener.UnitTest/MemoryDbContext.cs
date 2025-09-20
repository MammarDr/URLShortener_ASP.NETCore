using Castle.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UrlShortener.Core.Security.Authentication;
using UrlShortener.Core.Security.Identity;
using UrlShortener.Core.Services.Implementations;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data;
using UrlShortener.Data.Repositories;
using UrlShortener.Data.Repositories.Implementation;
using UrlShortener.Data.Repositories.Interfaces;
using UrlShortener.Models.Enums;
using Xunit.Abstractions;
using static UrlShortener.Extensions.ServiceExtensions;

namespace UrlShortener.UnitTest
{
    public class LazyService<T> : Lazy<T> where T : class
    {
        public LazyService(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>())
        {
        }
    }

    public class DepartmentAppServiceTest 
    {
        public IServiceProvider _provider; 
        public DepartmentAppServiceTest()
        {
            _provider = DiProvider.Provider;
        }

        [Fact]
        public void TestDependencyInjection()
        {
            // Get repository manager and validate repositories
            var repoManager = _provider.GetRequiredService<IRepositoryManager>();
            Assert.NotNull(repoManager.User);
            Assert.NotNull(repoManager.Url);
            Assert.NotNull(repoManager.Permission);
            Assert.NotNull(repoManager.Plan);
            Assert.NotNull(repoManager.RefreshToken);

            // Get service manager and validate services
            var serviceManager = _provider.GetRequiredService<IServiceManager>();
            Assert.NotNull(serviceManager.User);
            Assert.NotNull(serviceManager.UserService);
            Assert.NotNull(serviceManager.PermissionService);
            Assert.NotNull(serviceManager.PlanService);
            Assert.NotNull(serviceManager.AuthService);
            Assert.NotNull(serviceManager.TokenService);
            Assert.NotNull(serviceManager.ShortCodeService);

            // Get individual services directly
            var userService = _provider.GetRequiredService<IUserService>();
            var urlService = _provider.GetRequiredService<IUrlService>();
            var permissionService = _provider.GetRequiredService<IPermissionService>();
            var planService = _provider.GetRequiredService<IPlanService>();
            var shortCodeService = _provider.GetRequiredService<IShortCodeService>();
            var authService = _provider.GetRequiredService<IAuthService>();
            var tokenService = _provider.GetRequiredService<ITokenService>();
            var loggerManager = _provider.GetRequiredService<ILoggerManager>();

            Assert.NotNull(userService);
            Assert.NotNull(urlService);
            Assert.NotNull(permissionService);
            Assert.NotNull(planService);
            Assert.NotNull(shortCodeService);
            Assert.NotNull(authService);
            Assert.NotNull(tokenService);
            Assert.NotNull(loggerManager);
        }

    }

    public static class MoqServiceExtension
    {
        public static IServiceCollection AddRepositoryManager(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IUrlRepository, UrlRepository>();
            services.AddSingleton<IPermissionRepository, PermissionRepository>();
            services.AddSingleton<IPlanRepository, PlanRepository>();
            services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IShortCodeService, ShortCodeService>();
            services.AddSingleton<IRepositoryManager, RepositoryManager>();

            return services;
        }
        public static IServiceCollection AddServiceManager(this IServiceCollection services, UserContextAccessor userContext)
        {
            services.AddScoped(typeof(Lazy<>), typeof(LazyService<>));
            services.AddSingleton<IServiceManager, ServiceManager>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IUrlService, UrlService>();
            services.AddSingleton<IPermissionService, PermissionService>();
            services.AddSingleton<IPlanService, PlanService>();
            services.AddSingleton<IShortCodeService, ShortCodeService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ITokenService, TokenService>();
            

            services.AddSingleton<IUserContextAccessor>(userContext);

            return services;
        }

        public static IServiceCollection AddLoggerManager(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            return services;
        }

        public static IServiceCollection ConfigureJWT(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("unitTesting.json", optional: false, reloadOnChange: false)
            .Build();

            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

            var SigningKey = configuration.GetSection("Jwt:SigningKey").Value;

            /*if (jwtOptions is null)
                output.WriteLine("JWT Option are missing !!!!");

            if (string.IsNullOrEmpty(SigningKey))
                output.WriteLine("SigningKey is missing !!!!");*/

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
                });

            return services;
        }
    }

    public class DiProvider
    {
        private static IServiceProvider _provider;

        private static UserContextAccessor _userContext = new UserContextAccessor
            {
                CorrelationId = "911#551",
                Identity = new UserContext
                {
                    Id = 1,
                    Email = "email@gmail.com",
                    Permissions = new Dictionary<enResource, HashSet<enPermission>>
                    {
                        [enResource.User] = [enPermission.Create, enPermission.Read, enPermission.Update, enPermission.Delete],
                        [enResource.Url]  = [enPermission.Create, enPermission.Read, enPermission.Update, enPermission.Delete],
                        [enResource.Plan] = [enPermission.Create, enPermission.Read, enPermission.Update, enPermission.Delete],
                    }.AsReadOnly()
                }
            };

        public static IServiceProvider Provider
        {
            get
            {
                if (_provider == null)
                    _provider = CreateServiceProvider();
                return _provider;
            }
            set => _provider = value;
        }

        public static IServiceProvider CreateServiceProvider(string dbName = "TestDb", UserContextAccessor userContext = null)
        {
            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                                .UseInMemoryDatabase(dbName)
                                .Options;
     
            services.AddSingleton(options);
            services.AddSingleton<AppDbContext>();

            services.AddRepositoryManager()
                .AddServiceManager(userContext ?? _userContext)
                .ConfigureJWT()
                .AddLoggerManager();
            
            _provider = services.BuildServiceProvider();
            return _provider;
        }

        public static IServiceCollection CreateServiceCollection(string dbName = "TestDb", UserContextAccessor userContext = null)
        {
            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                                .UseInMemoryDatabase(dbName)
                                .Options;

            services.AddSingleton(options);
            services.AddSingleton<AppDbContext>();


            services.AddRepositoryManager()
                .AddServiceManager(userContext ?? _userContext)
                .AddLoggerManager();

            return services;
        }


    }




}

