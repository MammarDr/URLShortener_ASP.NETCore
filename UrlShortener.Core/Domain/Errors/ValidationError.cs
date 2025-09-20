using UrlShortener.Core.Services;

namespace UrlShortener.Core.Domain.Errors
{
    public sealed record ValidationError(IDictionary<string, List<string>> Errors,string Code, string? Description, enErrorType Type) : Error(Code, Description, Type);

}
