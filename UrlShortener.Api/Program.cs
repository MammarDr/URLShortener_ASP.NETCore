using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UrlShortener.Api.Autorization;
using UrlShortener.Api.Middlewares;
using UrlShortener.Core.Security.Authentication;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data;
using UrlShortener.Extensions;
using UrlShortener.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.RemoveGlobalModelValidator();

builder.Services.ConfigureLoggerService();
builder.Services.ConfigureUserContext();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureBusinessLayerServices();

DotNetEnv.Env.Load();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Information));


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.ConfigureJWT(builder.Configuration);


var validator = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(IObjectModelValidator));
if (validator != null)
{
    builder.Services.Remove(validator);
    builder.Services.Add(new ServiceDescriptor(typeof(IObjectModelValidator), _ => new EmptyModelValidator(), ServiceLifetime.Singleton));
}

NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
    .Services.BuildServiceProvider()
    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
    .OfType<NewtonsoftJsonPatchInputFormatter>().First();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity));
    options.Filters.Add<PermissionBasedAuthorizationFilter>();
    options.Filters.Add<ValidateModelFilter>();
    options.ModelBinderProviders.Insert(0, new ArrayModelBinderProvider());
    options.RespectBrowserAcceptHeader = true;
 //   options.ReturnHttpNotAcceptable = true;
    options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
 //   options.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
}).AddNewtonsoftJson();



builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Only disable returning 422 error, not really disabling global validation
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} else
{
    app.UseHsts();
}

    app.UseStaticFiles(); // enables using static files for the request.
                      // If  we don’t set a path to the static files directory,
                      // it will use a wwwroot folder in our project by default.

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ProfilingMiddlewarecs>();

app.UseHttpsRedirection();

app.UseIpRateLimiting();

app.UseCors("CorsPolicy"); // Must come after UseRouting and before UseAuthorization

app.UseResponseCaching();  // Below Cors
app.UseHttpCacheHeaders();

app.UseAuthentication(); // Populates HttpContext.User
app.UseAuthorization(); // Applies policies based on User

app.UseMiddleware<UserContextMiddleware>(); // Now safe to access claims after UseAuthentication

app.MapControllers();
//app.MapControllerRoute(name: "default", pattern: "v{version:apiVersion}/{controller}/{route}/{id?}");

app.Run();
