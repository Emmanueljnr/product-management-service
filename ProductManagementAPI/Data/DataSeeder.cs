using ProductManagementAPI.Models;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProductManagementAPI.Data
{
    public static class DataSeeder
    {
        public static void SeedData(IServiceProvider serviceProvider, IEnumerable<Product> products)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!context.Products.Any())
                {
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

                    foreach (var product in products)
                    {
                        // Remove any commas from the name and trim any whitespace
                        //product.Name = product.Name.Replace(",", "").Trim();

                        // Remove any commas from the name, trim any whitespace, and ensure each word starts with a capital letter
                        product.Name = textInfo.ToTitleCase(product.Name.Replace(",", "").Trim());

                        var nameWords = product.Name.Split(' ');
                        if (nameWords.Length > 5)
                        {
                            // Use the full name for the 'Details'
                            product.Details = product.Name;

                            // Truncate the 'Name' to the first 2 words
                            product.Name = string.Join(" ", nameWords.Take(2));
                        }
                        else if (string.IsNullOrEmpty(product.Details))
                        {
                            product.Details = "No details available...";
                        }

                        // Generate slug from the first 2 words of the name and append the product ID
                        product.Slug = GenerateUniqueSlug(context, string.Join("_", nameWords.Take(2)).ToLower(), product.Id);
                    }

                    context.Products.AddRange(products);
                    context.SaveChanges();
                }
            }
        }

        private static string GenerateUniqueSlug(ApplicationDbContext context, string baseSlug, Guid productId)
        {
            // Append the product ID to the base slug to ensure uniqueness
            return $"{baseSlug}_{productId.ToString().ToLower()}";
        }
    }

}

