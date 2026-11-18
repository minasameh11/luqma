namespace Luqma.Application.Features.Dtos.Requests.CategoryRequests
{
    public class SaveMainCategoryRequest
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;

        // When omitted or <= 0 the category is appended to the end of the list
        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
