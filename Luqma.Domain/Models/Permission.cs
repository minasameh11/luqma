namespace Luqma.Domain.Models
{
    public enum PermissionEnum
    {
        // Products
        CanViewProducts = 1,
        CanAddProducts = 2,
        CanEditProducts = 3,
        CanDeleteProducts = 4,

        // Categories
        CanViewCategories = 10,
        CanAddCategories = 11,
        CanEditCategories = 12,
        CanDeleteCategories = 13,

        // Orders
        CanViewOrders = 20,
        CanUpdateOrderStatus = 21,
        CanCancelOrders = 22,

        // Coupons
        CanViewCoupons = 30,
        CanManageCoupons = 31,

        // Users
        CanViewUsers = 40,
        CanManageUsers = 41,

        // Customization
        CanManageCustomization = 50,

        // Roles
        CanManageRoles = 60,
    }

    public class Permission : BaseEntity
    {
        public PermissionEnum PermissionType { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
        public Guid PermissionId { get; set; }
        public Permission? PermissionObj { get; set; }
    }
}
