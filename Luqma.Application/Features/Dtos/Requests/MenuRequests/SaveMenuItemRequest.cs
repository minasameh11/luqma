namespace Luqma.Application.Features.Dtos.Requests.MenuRequests
{
    public class SaveMenuItemRequest
    {
        public Guid MenuCategoryId { get; set; }

        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsSoldByWeight { get; set; }
        public decimal? WeightMin { get; set; }
        public decimal? WeightMax { get; set; }
        public decimal? WeightStep { get; set; }

        public bool IsActive { get; set; } = true;

        // Replaces the stored options/addons as a whole
        public List<SaveMenuItemOptionRequest> Options { get; set; } = new();
        public List<SaveMenuItemAddonRequest> Addons { get; set; } = new();
    }

    public class SaveMenuItemOptionRequest
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class SaveMenuItemAddonRequest
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
