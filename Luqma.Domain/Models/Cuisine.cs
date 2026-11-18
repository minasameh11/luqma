namespace Luqma.Domain.Models
{
    // A kitchen type a store serves, e.g. "مشويات" / "Grill" or "أسماك" / "Seafood".
    // A store can serve several cuisines, and customers filter stores by them.
    public class Cuisine : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
