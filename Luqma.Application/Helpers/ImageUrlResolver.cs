namespace Luqma.Application.Helpers
{
    public static class ImageUrlResolver
    {
        public static string ToAbsoluteUrl(string baseUrl, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(baseUrl))
                return url;

            if (Uri.TryCreate(url, UriKind.Absolute, out _))
                return url;

            return $"{baseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
        }
    }
}
