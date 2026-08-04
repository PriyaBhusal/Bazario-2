using System.Data.Entity;

namespace OnlineRetailStore.Mvc.Models
{
    public class AppDbContext : DbContext
    {
        // Connection string "MySqlOdbc" lives in Web.config and points at an existing
        // hand-managed schema (see SQL/mysql_setup.sql) - EF must never try to create
        // or migrate it.
        public AppDbContext() : base("name=MySqlOdbc")
        {
        }

        static AppDbContext()
        {
            Database.SetInitializer<AppDbContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
    }
}
