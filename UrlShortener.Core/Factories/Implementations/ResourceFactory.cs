using UrlShortener.Core.Factories.Interfaces;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Factories
{
    public class ResourceFactory : IResourceFactory
    {
        public enResource Create(string resource) => resource switch
        {
            "User" => enResource.User,
            "Url" => enResource.Url,
            "Plan" => enResource.Plan,
            _ => throw new InvalidCastException("'GetResourceFromString' Invalid Casting of Resource")
        };
    }

}
