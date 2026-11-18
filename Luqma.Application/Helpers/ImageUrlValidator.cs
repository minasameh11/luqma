namespace Luqma.Application.Helpers
{
    public static class ImageUrlValidator
    {
        // Accepts either a path served by the API (e.g. /uploads/...) or a full http(s) URL
        public static bool IsValidImageUrl(string iconUrl)
        {
            if (iconUrl.StartsWith('/'))
                return true;

            return Uri.TryCreate(iconUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
