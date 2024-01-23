using Newtonsoft.Json;
using ProductManagementAPI.DTOs;
using ProductManagementAPI.Models;
using System.Collections.Concurrent;

// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//                                                         CODE FOR THE Amazon API
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

// Ive added basic caching with a simple in-memory dictionary to store the results of previous searches.
namespace ProductManagementAPI.Services
{
    public class ExternalProductService
    {
        private readonly HttpClient _httpClient;
        private const string SearchUrl = "https://get-amazon-product.p.rapidapi.com/product/search";
        private static readonly ConcurrentDictionary<string, List<Product>> _cache = new ConcurrentDictionary<string, List<Product>>();

        /*
        public ExternalProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        */

        public ExternalProductService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<Product>> FetchProductsAsync(string keyword)
        {
            // Check cache first
            if (_cache.TryGetValue(keyword, out List<Product> cachedProducts))
            {
                return cachedProducts;
            }

            // Set up request headers
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{SearchUrl}/?keyword={keyword}&locale=gb"),
                Headers =
                {                      
                    { "X-RapidAPI-Key", "d0c2aab544msh3baf75545155e42p150842jsne55f747423c2" },
                    { "X-RapidAPI-Host", "get-amazon-product.p.rapidapi.com" },                                    
                },
            };

            // Send request
            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AmazonResponseDTO>(body);

                var mappedProducts = new List<Product>();
                foreach (var item in result.Data)
                {
                    var productImages = new List<string>();
                    if (item.Image != null)
                    {
                        if (!string.IsNullOrEmpty(item.Image.Primary))
                        {
                            productImages.Add(item.Image.Primary);
                        }
                        if (item.Image.Variants != null)
                        {
                            productImages.AddRange(item.Image.Variants);
                        }
                    }

                    var product = new Product
                    {
                        Id = Guid.NewGuid(), // Generate a new GUID for each product
                        Name = item.Title,
                        Price = item.Price,
                        Image = productImages, // Combine primary and variant images
                        // Additional properties like Details, Slug, etc. can be set here as needed
                    };

                    mappedProducts.Add(product);
                }

                // Cache the results
                _cache.TryAdd(keyword, mappedProducts);

                return mappedProducts;
            }
        }
    }

    // Helper DTOs to deserialize the response
    public class AmazonResponseDTO
    {
        public List<ExternalProductDTO> Data { get; set; }
    }
}




// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//                                                         CODE FOR THE 'undercutters' API
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/*
namespace ProductManagementAPI.Services
{
    public class ExternalProductService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://undercutters.azurewebsites.net/api"; // External API base URL.

        public ExternalProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IEnumerable<Product>> FetchProductsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/Product");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();       
            var externalProducts = JsonConvert.DeserializeObject<IEnumerable<ExternalProductDTO>>(content);

            // Map the external product data to your Product model.
            var mappedProducts = new List<Product>();

            foreach (var externalProduct in externalProducts)
            {
                var product = new Product
                {
                    // Assuming the external product has a similar structure
                    Id = Guid.NewGuid(), // Generate a new GUID for each product
                    Name = externalProduct.Name,
                    Details = externalProduct.Description,
                    Slug = externalProduct.Name.Replace(" ", "-").ToLower(),
                    Price = externalProduct.Price,
                    CreatedAt = DateTime.UtcNow, // Use actual date properties if available.
                    UpdatedAt = DateTime.UtcNow,
                    Image = new List<string> { "placeholder-image-url" } // Use a placeholder image URL.
                };

                mappedProducts.Add(product);
            }

            return mappedProducts;
        }
    }
}
*/

/*

This service class uses `HttpClient` to send a GET request to the external API, deserializes the JSON response into a collection of `ExternalProduct` objects, and then maps them to the `Product` model used in your application.

You'll need to add the placeholder image URL you want to use and adjust the property names and types according to the actual structure of the external API's response. Also, remember to register your `HttpClient` and the `ExternalProductService` in the `Startup.cs` or `Program.cs` file to use dependency injection.

Once you have this service set up and working, you can create controller actions that use this service to provide data to your front end. After that, you can proceed to deploy your application to Azure.

Make sure you test this locally with your front end to ensure that everything is working as expected before deployment. If you encounter any issues or need further assistance, please let me know.

*/