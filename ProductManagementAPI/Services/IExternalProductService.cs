using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services
{
    public interface IExternalProductService
    {
        Task<IEnumerable<Product>> FetchProductsAsync(string keyword);
    }
}
