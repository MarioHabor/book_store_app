using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using book_store_app_marian.Models;
using System.Reflection.Emit;

namespace book_store_app_marian.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categories> Categories { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Purchases> Purchases { get; set; }
        public DbSet<Reviews> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Categories>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(c => c.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            builder.Entity<Products>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(p => p.ProductName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(p => p.Price)
                      .IsRequired();
                entity.Property(p => p.CreatedTimestamp)
                      .IsRequired();
                entity.HasOne(p => p.Categories)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductImages>(entity =>
            {
                entity.ToTable("ProductImages");
                entity.HasKey(pi => pi.Id);
                entity.Property(pi => pi.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(pi => pi.ProductImage)
                      .IsRequired();
                entity.Property(pi => pi.MainImage)
                      .IsRequired();
                entity.Property(pi => pi.CreatedTimestamp)
                      .IsRequired();
                entity.HasOne(pi => pi.Products)
                      .WithMany(p => p.ProductImages)
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
