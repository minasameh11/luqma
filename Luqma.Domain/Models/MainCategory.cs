namespace Luqma.Domain.Models
{
    public class MainCategory : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        // Icon image shown for the category (relative path or absolute URL)
        public string IconUrl { get; set; } = null!;

        // Order the categories are displayed in on the client
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
