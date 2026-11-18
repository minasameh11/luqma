using Luqma.Domain.Helpers;
using Luqma.Domain.Interfaces;
using Luqma.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Luqma.Infrastructure.Database
{
    public class LuqmaContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly ICurrentUserService _currentUser;
        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ExternalLogin> ExternalLogins { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreImage> StoreImages { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemOption> MenuItemOptions { get; set; }
        public DbSet<MenuItemAddon> MenuItemAddons { get; set; }
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<StoreCuisine> StoreCuisines { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartItemAddon> CartItemAddons { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public LuqmaContext(DbContextOptions<LuqmaContext> options, ICurrentUserService currentUser) : base(options)
        {
            _currentUser = currentUser;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(LuqmaContext).Assembly);

            //Luqma.Infrastructure.Database.Config.GlobalFilterConfiguration.ApplyIsArchivedFilter(builder);

         
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entiries = ChangeTracker.Entries<BaseEntity>();
            var now = EgyptTime.Now;

            foreach (var entry in entiries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedById = _currentUser.UserId;
                        entry.Entity.CreatedByName = _currentUser.UserName;
                        entry.Entity.CreatedDateTime = now;
                        break;

                    case EntityState.Modified:
                        if (entry.Entity.IsArchived && entry.Entity.ArchivedDateTime == null)
                        {
                            entry.Entity.ArchivedById = _currentUser.UserId;
                            entry.Entity.ArchivedByName = _currentUser.UserName;
                            entry.Entity.ArchivedDateTime = now;
                        }
                        else
                        {
                            entry.Entity.ModifiedById = _currentUser.UserId;
                            entry.Entity.ModifiedByName = _currentUser.UserName;
                            entry.Entity.ModifiedDateTime = now;
                        }
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsArchived = true;
                        entry.Entity.ArchivedById = _currentUser.UserId;
                        entry.Entity.ArchivedByName = _currentUser.UserName;
                        entry.Entity.ArchivedDateTime = now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
