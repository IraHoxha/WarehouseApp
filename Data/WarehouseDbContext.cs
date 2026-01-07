using Microsoft.EntityFrameworkCore;
using warehouseapp.Data.Models;

namespace warehouse.Data
{
    public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Partner> Partners { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagValue> TagValues { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<TagValue>()
                .HasIndex(tv => new { tv.TagId, tv.Value })
                .IsUnique();

            modelBuilder.Entity<TagValue>()
                .HasOne(tv => tv.Tag)
                .WithMany(t => t.Values)
                .HasForeignKey(tv => tv.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany()
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.TagValue)
                .WithMany()
                .HasForeignKey(pt => pt.TagValueId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Partner)
                .WithMany()
                .HasForeignKey(it => it.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Order)
                .WithMany(o => o.InventoryTransactions)
                .HasForeignKey(it => it.OrderId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.UnitCostPrice)
                      .HasPrecision(18, 4);

                entity.Property(p => p.UnitSellingPrice)
                      .HasPrecision(18, 4);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(i => i.UnitSellingPrice)
                      .HasPrecision(18, 4);
            });

            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.Property(t => t.UnitPrice)
                      .HasPrecision(18, 4);
            });


            modelBuilder.Entity<Partner>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

            modelBuilder.Entity<Category>()
            .HasIndex(c => new { c.ParentCategoryId, c.Name })
            .IsUnique();
        }
    }
}
