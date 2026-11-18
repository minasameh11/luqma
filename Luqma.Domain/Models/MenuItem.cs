namespace Luqma.Domain.Models
{
    // A sellable item inside a menu category, e.g. "مشاوي مشكلة"
    public class MenuItem : BaseEntity
    {
        public Guid MenuCategoryId { get; set; }
        public MenuCategory? MenuCategory { get; set; }

        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }

        // Item image (relative path or absolute URL)
        public string? ImageUrl { get; set; }

        // Sold by weight (e.g. kebab sold per kilo) instead of per unit
        public bool IsSoldByWeight { get; set; }
        public decimal? WeightMin { get; set; }
        public decimal? WeightMax { get; set; }
        public decimal? WeightStep { get; set; }

        public bool IsActive { get; set; } = true;

        // Choices for the item, e.g. "نفر واحد" / "نفرين"
        public ICollection<MenuItemOption> Options { get; set; } = new List<MenuItemOption>();

        // Extra paid additions, e.g. "إضافة أرز"
        public ICollection<MenuItemAddon> Addons { get; set; } = new List<MenuItemAddon>();
    }
}
