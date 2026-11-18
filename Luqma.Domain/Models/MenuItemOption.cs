namespace Luqma.Domain.Models
{
    public class MenuItemOption : BaseEntity
    {
        public Guid MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        // Extra price added on top of the item price
        public decimal Price { get; set; }
    }
}
