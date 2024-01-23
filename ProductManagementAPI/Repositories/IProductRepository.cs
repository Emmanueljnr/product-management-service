using ProductManagementAPI.Models;

namespace ProductManagementAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductBySlugAsync(string slug);
        Task<Product> GetProductByIdAsync(Guid id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid id);

    }
}
