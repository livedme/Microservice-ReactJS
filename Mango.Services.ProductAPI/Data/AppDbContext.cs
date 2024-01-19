using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        
        }

        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product()
            {
                ProductId = 1,
                Name = "Product-1",
                Description ="",
                CategoryName= "Category-1",
                ImageUrl=""
            });
            modelBuilder.Entity<Product>().HasData(new Product()
            {
                ProductId = 2,
                Name = "Product-2",
                Description = "",
                CategoryName = "Category-2",
                ImageUrl = ""
            });
        }
    }
}
