using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Repositories;
using ProductManagementAPI.Services;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Endpoint to get products from the database
        [HttpGet("database")]
        public async Task<IActionResult> GetFromDatabase()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return Ok(products);
        }


        // Endpoint to get a single product by its slug
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var product = await _productRepository.GetProductBySlugAsync(slug);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound();
        }
    }

}


/*
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< My Original Controller (Gets Products from the Amazon API Call and sends them to the front end) >>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ExternalProductService _externalProductService;

        public ProductsController(ExternalProductService externalProductService)
        {
            _externalProductService = externalProductService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // You could alternate between different search keywords here or use a query parameter to specify the keyword.
            var headphones = await _externalProductService.FetchProductsAsync("Active Noise Cancelling Wireless Headphones, Over- Ear Bluetooth Headphones, milky white, gold,"); //headphones
            var earphones = await _externalProductService.FetchProductsAsync("Wireless Earbuds, In Ear with Noise Cancelling Mic, 2023 Bluetooth Earphones Mini HI-FI Stereo Sound, LED Display gold green pink"); //earphones

            // Combine the lists and return them.
            var products = headphones.Concat(earphones).ToList();
            return Ok(products);
        }
    }
*/





// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//                                                         CODE FOR THE 'undercutters' API
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
/*
     [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ExternalProductService _externalProductService;

        public ProductsController(ExternalProductService externalProductService)
        {
            _externalProductService = externalProductService;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _externalProductService.FetchProductsAsync();
            return Ok(products);
        }
    }
*/
