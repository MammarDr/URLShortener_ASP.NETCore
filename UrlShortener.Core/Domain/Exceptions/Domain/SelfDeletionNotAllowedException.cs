using UrlShortener.Core.Domain.Exceptions.Base;

namespace UrlShortener.Core.Domain.Exceptions.Domain
{
    public class SelfDeletionNotAllowedException : DomainException
    {
        public SelfDeletionNotAllowedException(string message = "Managers cannot delete their own accounts.") : base(message) { }
    }
}
