using Luqma.Application.Features.Dtos.Requests.CategoryRequests;

namespace Luqma.Application.Features.Commands.CategoryCommands
{
    public static class MainCategoryRequestValidator
    {
        public const int MaxNameLength = 100;
        public const int MaxIconUrlLength = 500;

        public static string? Validate(SaveMainCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NameAr) || request.NameAr.Trim().Length > MaxNameLength)
                return "اسم الصنف بالعربية مطلوب ويجب ألا يزيد عن 100 حرف";

            if (string.IsNullOrWhiteSpace(request.NameEn) || request.NameEn.Trim().Length > MaxNameLength)
                return "اسم الصنف بالإنجليزية مطلوب ويجب ألا يزيد عن 100 حرف";

            if (string.IsNullOrWhiteSpace(request.IconUrl) || request.IconUrl.Trim().Length > MaxIconUrlLength)
                return "صورة الأيقونة مطلوبة ويجب ألا يزيد طول الرابط عن 500 حرف";

            if (!IsValidIconUrl(request.IconUrl.Trim()))
                return "رابط صورة الأيقونة غير صحيح";

            return null;
        }

        private static bool IsValidIconUrl(string iconUrl)
        {
            if (iconUrl.StartsWith('/'))
                return true;

            return Uri.TryCreate(iconUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
