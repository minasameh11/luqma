using Luqma.Application.Features.Dtos.Requests.StoreRequests;
using Luqma.Application.Helpers;
using Luqma.Domain.Enums;
using Luqma.Domain.Helpers;
using System.Text.RegularExpressions;

namespace Luqma.Application.Features.Commands.StoreCommands
{
    public static class StoreRequestValidator
    {
        public const int MaxNameLength = 100;
        public const int MaxPhoneLength = 20;
        public const int MaxAddressLength = 300;
        public const int MaxIconUrlLength = 500;
        public const int MaxDescriptionLength = 1000;
        public const int MaxImageUrlLength = 500;
        public const int MaxImages = 10;
        public const int MaxDeliveryTimeMinutes = 180;
        public const decimal MaxDeliveryFee = 10000m;

        private static readonly Regex PhonePattern = new(@"^[0-9+\-\s()]{7,20}$", RegexOptions.Compiled);

        public static string? Validate(SaveStoreRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NameAr) || request.NameAr.Trim().Length > MaxNameLength)
                return "اسم المحل بالعربية مطلوب ويجب ألا يزيد عن 100 حرف";

            if (string.IsNullOrWhiteSpace(request.NameEn) || request.NameEn.Trim().Length > MaxNameLength)
                return "اسم المحل بالإنجليزية مطلوب ويجب ألا يزيد عن 100 حرف";

            var phone = request.PhoneNumber?.Trim() ?? string.Empty;
            if (phone.Length == 0 || !PhonePattern.IsMatch(phone))
                return "رقم التليفون مطلوب ويجب أن يتكون من أرقام ورموز + - ( ) بحد أقصى 20 خانة";

            if (string.IsNullOrWhiteSpace(request.Address) || request.Address.Trim().Length > MaxAddressLength)
                return "عنوان المحل مطلوب ويجب ألا يزيد عن 300 حرف";

            if (string.IsNullOrWhiteSpace(request.IconUrl) || request.IconUrl.Trim().Length > MaxIconUrlLength)
                return "صورة الأيقونة مطلوبة ويجب ألا يزيد طول الرابط عن 500 حرف";

            if (!ImageUrlValidator.IsValidImageUrl(request.IconUrl.Trim()))
                return "رابط صورة الأيقونة غير صحيح";

            if (request.MainCategoryId == Guid.Empty)
                return "التصنيف مطلوب";

            var descriptionError = ValidateDescription(request.DescriptionAr, request.DescriptionEn);
            if (descriptionError != null)
                return descriptionError;

            if (!Enum.IsDefined(typeof(PriceLevel), request.PriceLevel))
                return "مستوى الأسعار غير صحيح";

            if (request.DeliveryTimeMinutes is < 0 or > MaxDeliveryTimeMinutes)
                return "وقت التوصيل يجب أن يكون بين 0 و180 دقيقة";

            if (request.DeliveryFee < 0 || request.DeliveryFee > MaxDeliveryFee)
                return "رسوم التوصيل يجب أن تكون بين 0 و10000";

            var locationError = ValidateLocation(request.Latitude, request.Longitude);
            if (locationError != null)
                return locationError;

            return ValidateImages(request.ImageUrls);
        }

        public static string? ValidateDescription(string? descriptionAr, string? descriptionEn)
        {
            if (descriptionAr != null && descriptionAr.Trim().Length > MaxDescriptionLength)
                return "وصف المحل بالعربية يجب ألا يزيد عن 1000 حرف";

            if (descriptionEn != null && descriptionEn.Trim().Length > MaxDescriptionLength)
                return "وصف المحل بالإنجليزية يجب ألا يزيد عن 1000 حرف";

            return null;
        }

        public static string? ValidateLocation(double latitude, double longitude)
        {
            // (0,0) means the location was not set; any other value must be a real map point
            if (latitude == 0 && longitude == 0)
                return null;

            if (!GeoDistance.IsSet(latitude, longitude))
                return "إحداثيات الموقع غير صحيحة";

            return null;
        }

        public static string? ValidateImages(List<string>? imageUrls)
        {
            if (imageUrls == null)
                return null;

            if (imageUrls.Count > MaxImages)
                return $"عدد صور المحل يجب ألا يزيد عن {MaxImages} صور";

            foreach (var imageUrl in imageUrls)
            {
                if (string.IsNullOrWhiteSpace(imageUrl) || imageUrl.Trim().Length > MaxImageUrlLength)
                    return "رابط صورة المحل غير صحيح";

                if (!ImageUrlValidator.IsValidImageUrl(imageUrl.Trim()))
                    return "رابط صورة المحل غير صحيح";
            }

            return null;
        }
    }
}
