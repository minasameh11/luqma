using Luqma.Application.Features.Dtos.Requests.MenuRequests;
using Luqma.Application.Helpers;

namespace Luqma.Application.Features.Commands.MenuCommands
{
    public static class MenuRequestValidator
    {
        public const int MaxNameLength = 150;
        public const int MaxCategoryNameLength = 100;
        public const int MaxDescriptionLength = 1000;
        public const int MaxImageUrlLength = 500;
        public const int MaxOptions = 20;
        public const int MaxAddons = 20;

        public static string? ValidateCategory(SaveMenuCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NameAr) || request.NameAr.Trim().Length > MaxCategoryNameLength)
                return "اسم القسم بالعربية مطلوب ويجب ألا يزيد عن 100 حرف";

            if (string.IsNullOrWhiteSpace(request.NameEn) || request.NameEn.Trim().Length > MaxCategoryNameLength)
                return "اسم القسم بالإنجليزية مطلوب ويجب ألا يزيد عن 100 حرف";

            if (request.ImageUrl is { Length: > MaxImageUrlLength })
                return "رابط صورة القسم يجب ألا يزيد عن 500 حرف";

            if (!string.IsNullOrWhiteSpace(request.ImageUrl) &&
                !ImageUrlValidator.IsValidImageUrl(request.ImageUrl.Trim()))
                return "رابط صورة القسم غير صحيح";

            return null;
        }

        public static string? ValidateItem(SaveMenuItemRequest request)
        {
            if (request.MenuCategoryId == Guid.Empty)
                return "القسم مطلوب";

            if (string.IsNullOrWhiteSpace(request.NameAr) || request.NameAr.Trim().Length > MaxNameLength)
                return "اسم الصنف بالعربية مطلوب ويجب ألا يزيد عن 150 حرف";

            if (string.IsNullOrWhiteSpace(request.NameEn) || request.NameEn.Trim().Length > MaxNameLength)
                return "اسم الصنف بالإنجليزية مطلوب ويجب ألا يزيد عن 150 حرف";

            if (request.DescriptionAr is { Length: > MaxDescriptionLength } ||
                request.DescriptionEn is { Length: > MaxDescriptionLength })
                return "وصف الصنف يجب ألا يزيد عن 1000 حرف";

            if (request.Price <= 0)
                return "سعر الصنف يجب أن يكون أكبر من صفر";

            if (request.CompareAtPrice.HasValue && request.CompareAtPrice.Value <= request.Price)
                return "السعر قبل الخصم يجب أن يكون أكبر من السعر الحالي";

            if (request.ImageUrl is { Length: > MaxImageUrlLength })
                return "رابط صورة الصنف يجب ألا يزيد عن 500 حرف";

            if (!string.IsNullOrWhiteSpace(request.ImageUrl) &&
                !ImageUrlValidator.IsValidImageUrl(request.ImageUrl.Trim()))
                return "رابط صورة الصنف غير صحيح";

            if (request.IsSoldByWeight)
            {
                if (!request.WeightMin.HasValue || !request.WeightMax.HasValue || !request.WeightStep.HasValue)
                    return "الحد الأدنى والأقصى والخطوة مطلوبة للصنف المُباع بالوزن";

                if (request.WeightMin < 0 || request.WeightMax <= request.WeightMin || request.WeightStep <= 0)
                    return "قيم الوزن غير صحيحة";
            }

            if (request.Options.Count > MaxOptions)
                return $"لا يمكن إضافة أكثر من {MaxOptions} اختيار للصنف";

            foreach (var option in request.Options)
            {
                if (string.IsNullOrWhiteSpace(option.NameAr) ||
                    option.NameAr.Trim().Length > MaxCategoryNameLength ||
                    string.IsNullOrWhiteSpace(option.NameEn) ||
                    option.NameEn.Trim().Length > MaxCategoryNameLength)
                    return "اسم الاختيار مطلوب بالعربية والإنجليزية ويجب ألا يزيد عن 100 حرف";

                if (option.Price < 0)
                    return "سعر الاختيار لا يمكن أن يكون سالبًا";
            }

            if (request.Addons.Count > MaxAddons)
                return $"لا يمكن إضافة أكثر من {MaxAddons} إضافة للصنف";

            foreach (var addon in request.Addons)
            {
                if (string.IsNullOrWhiteSpace(addon.NameAr) ||
                    addon.NameAr.Trim().Length > MaxCategoryNameLength ||
                    string.IsNullOrWhiteSpace(addon.NameEn) ||
                    addon.NameEn.Trim().Length > MaxCategoryNameLength)
                    return "اسم الإضافة مطلوب بالعربية والإنجليزية ويجب ألا يزيد عن 100 حرف";

                if (addon.Price < 0)
                    return "سعر الإضافة لا يمكن أن يكون سالبًا";

                if (addon.ImageUrl is { Length: > MaxImageUrlLength })
                    return "رابط صورة الإضافة يجب ألا يزيد عن 500 حرف";
            }

            return null;
        }
    }
}
