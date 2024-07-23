using Microsoft.EntityFrameworkCore;

namespace SE172266.ProductManagement.Repo.Entities
{
    public class BookStoreODataDBContext : DbContext
    {
        public BookStoreODataDBContext(DbContextOptions<BookStoreODataDBContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Press> Presses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;database=BookStoreODataDB;TrustServerCertificate=True");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().OwnsOne(c => c.Location);
            modelBuilder.Entity<Press>().HasData(new Press
            {
                Id = 1,
                Name = "Test",
                Category = Category.Book
            });
        }
    }
}
