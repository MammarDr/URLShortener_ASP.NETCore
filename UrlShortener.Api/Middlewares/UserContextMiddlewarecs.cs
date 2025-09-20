using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System.Net;
using System.Security.Claims;
using UrlShortener.Core.Factories;
using UrlShortener.Core.Security.Identity;

namespace UrlShortener.Api.Middlewares
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserContextAccessor user)
        {
            
            var ClaimIdentity = context.User.Identity as ClaimsIdentity;
            

            if (ClaimIdentity is not null && ClaimIdentity.IsAuthenticated) {

                
                var ResourceFactory = new ResourceFactory();
                var PermissionFactory = new PermissionFactory();

                var permissions = context.User.Claims.Where(c => c.Type.StartsWith("perm."))
                    .GroupBy(c => c.Type[5..])
                    .ToDictionary(
                        group => ResourceFactory.Create(group.Key),
                        group => group.Select(c => PermissionFactory.Create(c.Value)).ToHashSet()
                    );

                int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);

                user.Identity = new UserContext
               {
                   Id = userId,
                   Email = context.User.FindFirstValue(ClaimTypes.Email)!,
                   Permissions = permissions.AsReadOnly()
               };

                if (user.Identity.Id == 0 || user.Identity.Email is null)
                {
                    context.Response.StatusCode = 401;
                    context.Response.Headers["X-Auth-Error"] = "token_expired";
                    await context.Response.WriteAsync("Authentication required.");
                    return;
                }         
            }

            user.CorrelationId = context.TraceIdentifier ?? Guid.NewGuid().ToString();

            using (NLog.ScopeContext.PushProperty("CorrelationId", user.CorrelationId))
            {
                await _next(context); 
            }

        }
    }
}
