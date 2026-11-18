namespace Luqma.Domain.Models
{
    // Links a store to one of the cuisines it serves
    public class StoreCuisine : BaseEntity
    {
        public Guid StoreId { get; set; }
        public Store? Store { get; set; }

        public Guid CuisineId { get; set; }
        public Cuisine? Cuisine { get; set; }
    }
}
