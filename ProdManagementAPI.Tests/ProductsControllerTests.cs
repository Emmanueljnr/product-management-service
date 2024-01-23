using Xunit;
using Moq;
using ProductManagementAPI.Controllers;
using ProductManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagementAPI.Models; 

public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _controller = new ProductsController(_mockRepo.Object);
    }

    [Fact]
    public async Task GetFromDatabase_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllProductsAsync())
            .ReturnsAsync(GetTestProducts());

        // Act
        var result = await _controller.GetFromDatabase();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value); 
        var productList = returnValue.ToList();
        Assert.Equal(2, productList.Count); 
    }

    [Fact]
    public async Task GetProductBySlug_ReturnsOkResult_WithProduct_WhenProductExists()
    {
        // Arrange
        string existingSlug = "product-1";
        var product = GetTestProducts().FirstOrDefault(p => p.Slug == existingSlug);
        _mockRepo.Setup(repo => repo.GetProductBySlugAsync(existingSlug))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProductBySlug(existingSlug);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Product>(okResult.Value); 
        Assert.Equal(product.Name, returnValue.Name);
        Assert.Equal(product.Price, returnValue.Price);
    }

    // Helper method to create a list of Product objects
    private IEnumerable<Product> GetTestProducts()
    {
        return new List<Product>
        {
            new Product { Name = "Product 1", Price = 10, Slug = "product-1", Details = "Details1", Image = new List<string>() },
            new Product { Name = "Product 2", Price = 20, Slug = "product-2", Details = "Details2", Image = new List<string>() }
        };
    }
}
