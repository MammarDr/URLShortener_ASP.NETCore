using UrlShortener.Models.Enums;

namespace UrlShortener.Api.Autorization
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CheckPermissionAttribute : Attribute
    {
        public CheckPermissionAttribute(enResource res, enPermission perm)
        {
            Permission = perm;
            Resource   = res;
        }

        public enPermission Permission { get; }
        public enResource   Resource   { get; }
    }
}
