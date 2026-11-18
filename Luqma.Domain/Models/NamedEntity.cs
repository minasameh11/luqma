namespace Luqma.Domain.Models
{
    public abstract class NamedEntity : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
    }
}
