namespace Luqma.Domain.Models
{
    // A menu section inside a single store, e.g. "مشاوي" / "Grill"
    public class MenuCategory : BaseEntity
    {
        public Guid StoreId { get; set; }
        public Store? Store { get; set; }

        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        // Section image shown on the menu (relative path or absolute URL)
        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}
