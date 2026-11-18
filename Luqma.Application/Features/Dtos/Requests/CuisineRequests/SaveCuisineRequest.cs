namespace Luqma.Application.Features.Dtos.Requests.CuisineRequests
{
    public class SaveCuisineRequest
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        // When omitted or <= 0 the cuisine is appended to the end of the list
        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
