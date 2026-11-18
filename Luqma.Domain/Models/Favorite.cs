namespace Luqma.Domain.Models
{
    // A menu item saved by a user; the same item is saved once per user
    public class Favorite : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }
    }
}
