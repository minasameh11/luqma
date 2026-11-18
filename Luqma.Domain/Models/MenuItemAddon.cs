namespace Luqma.Domain.Models
{
    public class MenuItemAddon : BaseEntity
    {
        public Guid MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
    }
}
