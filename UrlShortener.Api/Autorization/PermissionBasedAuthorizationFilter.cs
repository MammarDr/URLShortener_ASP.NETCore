using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UrlShortener.Core.Security.Identity;
using UrlShortener.Models.Enums;


namespace UrlShortener.Api.Autorization
{
    public class PermissionBasedAuthorizationFilter(IUserContextAccessor userContext) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is CheckPermissionAttribute) as CheckPermissionAttribute;

            if (attribute is null)
                return;

            if (userContext.Identity is null || !userContext.Identity.Permissions.ContainsKey(attribute.Resource))
            {
                context.Result = new ForbidResult();
                return;
            }
    
            enPermission perm = 0;

            foreach(var item in userContext.Identity.Permissions[attribute.Resource])
            {
                perm |= item;
            }

            if((perm & attribute.Permission) != attribute.Permission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
