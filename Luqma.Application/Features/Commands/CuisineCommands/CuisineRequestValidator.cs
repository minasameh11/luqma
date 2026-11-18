using Luqma.Application.Features.Dtos.Requests.CuisineRequests;
using Luqma.Application.Helpers;

namespace Luqma.Application.Features.Commands.CuisineCommands
{
    public static class CuisineRequestValidator
    {
        public const int MaxNameLength = 100;
        public const int MaxImageUrlLength = 500;

        public static string? Validate(SaveCuisineRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NameAr) || request.NameAr.Trim().Length > MaxNameLength)
                return "اسم نوع الأكل بالعربية مطلوب ويجب ألا يزيد عن 100 حرف";

            if (string.IsNullOrWhiteSpace(request.NameEn) || request.NameEn.Trim().Length > MaxNameLength)
                return "اسم نوع الأكل بالإنجليزية مطلوب ويجب ألا يزيد عن 100 حرف";

            if (request.ImageUrl is { Length: > MaxImageUrlLength })
                return "رابط الصورة يجب ألا يزيد عن 500 حرف";

            if (!string.IsNullOrWhiteSpace(request.ImageUrl) &&
                !ImageUrlValidator.IsValidImageUrl(request.ImageUrl.Trim()))
                return "رابط الصورة غير صحيح";

            return null;
        }
    }
}
