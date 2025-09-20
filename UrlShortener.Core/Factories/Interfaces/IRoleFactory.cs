using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Factories.Interfaces
{
    public interface IRoleFactory
    {
        enRole Create(string resource);
        string GetRoleID(enRole role);
    }

}
