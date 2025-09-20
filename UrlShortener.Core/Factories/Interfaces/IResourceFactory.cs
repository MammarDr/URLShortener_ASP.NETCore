using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Factories.Interfaces
{
    public interface IResourceFactory
    {
        enResource Create(string resource);
    }

}
