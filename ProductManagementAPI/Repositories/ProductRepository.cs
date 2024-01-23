using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Data;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductBySlugAsync(string slug)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await GetProductByIdAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
