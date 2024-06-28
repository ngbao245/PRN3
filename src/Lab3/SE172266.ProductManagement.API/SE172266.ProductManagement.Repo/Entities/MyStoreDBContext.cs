using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace SE172266.ProductManagement.Repo.Entities
{
    public class MyStoreDBContext : IdentityDbContext<IdentityUser>
    {
        public MyStoreDBContext(DbContextOptions options) : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(x => x.IsDeleted == false);
            modelBuilder.Entity<Category>().HasQueryFilter(x => x.IsDeleted == false);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Beverages" },
            new Category { CategoryId = 2, CategoryName = "Condiments" },
            new Category { CategoryId = 3, CategoryName = "Confections" },
            new Category { CategoryId = 4, CategoryName = "Dairy Products" },
            new Category { CategoryId = 5, CategoryName = "Grains/Cereals" },
            new Category { CategoryId = 6, CategoryName = "Meat/Poultry" },
            new Category { CategoryId = 7, CategoryName = "Produce" },
            new Category { CategoryId = 8, CategoryName = "Seafood" });

            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                ProductId = 1,
                ProductName = "Pizza",
                CategoryId = 1,
                UnitsInStock = 1,
                UnitPrice = 5,
            },
            new Product
            {
                ProductId = 2,
                ProductName = "Hamburger",
                CategoryId = 2,
                UnitsInStock = 2,
                UnitPrice = 6,
            },
            new Product
            {
                ProductId = 3,
                ProductName = "Sushi",
                CategoryId = 3,
                UnitsInStock = 3,
                UnitPrice = 7,
            },
            new Product
            {
                ProductId = 5,
                ProductName = "Fried Chicken",
                CategoryId = 5,
                UnitsInStock = 5,
                UnitPrice = 8,
            },
            new Product
            {
                ProductId = 6,
                ProductName = "Salmon",
                CategoryId = 6,
                UnitsInStock = 6,
                UnitPrice = 8,
            },
            new Product
            {
                ProductId = 7,
                ProductName = "Steak",
                CategoryId = 7,
                UnitsInStock = 7,
                UnitPrice = 9,
            });
        }
    }
}
