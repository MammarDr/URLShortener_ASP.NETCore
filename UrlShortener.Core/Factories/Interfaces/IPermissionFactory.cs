using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Factories.Interfaces
{
    public interface IPermissionFactory
    {
        enPermission Create(string permission);
    }

}
