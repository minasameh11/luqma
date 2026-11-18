namespace Luqma.Application.Features.Dtos.Requests.MenuRequests
{
    public class SaveMenuCategoryRequest
    {
        // Required on create only; an existing category cannot be moved to another store
        public Guid? StoreId { get; set; }

        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        // When omitted or <= 0 the category is appended to the end of the menu
        public int? DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
