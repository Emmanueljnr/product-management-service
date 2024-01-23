using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define a value converter for a list of strings.
            var splitStringConverter = new ValueConverter<List<string>, string>(
                v => string.Join(';', v), // Convert the list to a single string with ';' as a separator.
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()); // Convert the string back to a list.

            // Tell EF Core to use the converter for the 'Image' property on the 'Product' entity.
            modelBuilder.Entity<Product>()
                .Property(p => p.Image)
                .HasConversion(splitStringConverter)
                .HasColumnName("Image") // The name of the column in the database.
                .HasColumnType("text"); // Use an appropriate data type that can hold the combined string.
        }

    }
}

