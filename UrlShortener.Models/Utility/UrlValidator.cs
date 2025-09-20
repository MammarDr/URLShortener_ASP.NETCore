using System.Text.RegularExpressions;


namespace UrlShortener.Core.Validators
{
    public static class URLValidator
    {
        public static bool Validate(string url)
        {
            if(string.IsNullOrEmpty(url)) return false;

            Regex regex =
                new Regex("^(?:https?:\\/\\/)?(?:www\\.)?(?:[\\w-]+\\.)+(?:\\d+|(?:[a-zA-Z]{2,})(?::\\d+|(?:\\/(\\w+)?)*(?:\\.\\w+)?\\??(?:[\\w%~+-]+=[\\w%~+-]+&?)*(?:\\w+)?(?:#$|#\\w+)?)*)");

            return regex.IsMatch(url);
        }
    }
}
