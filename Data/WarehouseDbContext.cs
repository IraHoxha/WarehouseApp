using Microsoft.EntityFrameworkCore;
using warehouseapp.Data.Models;

namespace warehouse.Data
{
    public class WarehouseDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Partner)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);


            // globaldecimal precision
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(18);
                        property.SetScale(4);
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
