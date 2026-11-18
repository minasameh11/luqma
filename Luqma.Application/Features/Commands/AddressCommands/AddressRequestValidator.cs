using Luqma.Application.Features.Dtos.Requests.AddressRequests;

namespace Luqma.Application.Features.Commands.AddressCommands
{
    public static class AddressRequestValidator
    {
        public const int MaxAddressesPerUser = 10;

        public static string? Validate(SaveAddressRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length > 200)
                return "عنوان الشارع مطلوب ويجب ألا يزيد عن 200 حرف";

            if (string.IsNullOrWhiteSpace(request.City) || request.City.Trim().Length > 100)
                return "المدينة مطلوبة ويجب ألا تزيد عن 100 حرف";

            if (request.District is { Length: > 100 })
                return "اسم الحي يجب ألا يزيد عن 100 حرف";

            if (request.Details is { Length: > 500 })
                return "التفاصيل يجب ألا تزيد عن 500 حرف";

            if (!Enum.IsDefined(request.Label))
                return "نوع العنوان غير صحيح";

            if (double.IsNaN(request.Latitude) || request.Latitude is < -90 or > 90 ||
                double.IsNaN(request.Longitude) || request.Longitude is < -180 or > 180)
                return "إحداثيات الموقع غير صحيحة";

            return null;
        }
    }
}
