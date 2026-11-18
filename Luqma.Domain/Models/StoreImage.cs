namespace Luqma.Domain.Models
{
    // One photo in the store gallery; the icon is stored on the store itself
    public class StoreImage : BaseEntity
    {
        public Guid StoreId { get; set; }
        public Store? Store { get; set; }

        // Relative path (e.g. /uploads/stores/...) or a full http(s) URL
        public string ImageUrl { get; set; } = null!;

        public int DisplayOrder { get; set; }
    }
}
