using Xunit;
using Moq;
using ProductManagementAPI.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProductManagementAPI.Models;
using System.Threading;

namespace ProdManagementAPI.Tests
{
    public class DailyUpdateServiceTests
    {
        [Fact]
        public async Task FetchAndUpdateData_WritesToJSONFile()
        {
            // Arrange
            //var mockExternalProductService = new Mock<ExternalProductService>
            var mockExternalProductService = new Mock<IExternalProductService>();
            var mockFileService = new Mock<IFileService>();
            var dummyProducts = new List<Product>
            {
                new Product { Name = "Dummy Product 1", Price = 9.99m },
                new Product { Name = "Dummy Product 2", Price = 19.99m }
            };

            mockExternalProductService.Setup(service => service.FetchProductsAsync(It.IsAny<string>()))
                .ReturnsAsync(dummyProducts);

            mockFileService.Setup(fs => fs.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable("The file write method was not called");

            var service = new DailyUpdateService(mockExternalProductService.Object, mockFileService.Object);

            // Act
            await service.FetchAndUpdateData(); // This method needs to be public or internal for testing

            // Assert
            mockFileService.Verify(); // Verify that IFileService.WriteAllTextAsync was called
        }



        [Fact]
        public async Task ExecuteAsync_InvokesFetchAndUpdateData()
        {
            // Arrange
            var mockExternalProductService = new Mock<IExternalProductService>();
            var mockFileService = new Mock<IFileService>();
            var cts = new CancellationTokenSource();
            var expectedProducts = new List<Product>
            {
                new Product { Name = "Dummy Product 1", Price = 10, Slug = "product-1", Details = "Details1", Image = new List<string>() },
                new Product { Name = "Dummy Product 2", Price = 20, Slug = "product-2", Details = "Details2", Image = new List<string>() }
            };

            mockExternalProductService.Setup(service => service.FetchProductsAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedProducts)
                .Verifiable(); // Make sure it's verifiable

            var service = new DailyUpdateService(mockExternalProductService.Object, mockFileService.Object);

            // Act
            var executeTask = service.StartAsync(cts.Token);

            // Simulate some delay for the service to start
            await Task.Delay(1000);
            // Now cancel the service to simulate stopping
            cts.Cancel();

            // This is a way to allow any ongoing work to complete before assertions
            await executeTask;

            // Assert
            mockExternalProductService.Verify(); // This verifies that FetchProductsAsync was called
        }



        /*
        [Fact]
        public async Task ExecuteAsync_InvokesFetchAndUpdateData()
        {
            // Arrange
            //var mockExternalProductService = new Mock<ExternalProductService>
            var mockExternalProductService = new Mock<IExternalProductService>();
            var mockFileService = new Mock<IFileService>();
            var cts = new CancellationTokenSource();

            var service = new DailyUpdateService(mockExternalProductService.Object, mockFileService.Object);

            // Act
            var executeTask = service.StartAsync(cts.Token);

            // Simulate some delay for the service to start
            await Task.Delay(1000);
            // Now cancel the service to simulate stopping
            cts.Cancel();

            // This is a way to allow any ongoing work to complete before assertions
            await executeTask;

            // Assert
            // Assertions here would be indirect; we could check if FetchProductsAsync was called
            mockExternalProductService.Verify(service => service.FetchProductsAsync(It.IsAny<string>()), Times.AtLeastOnce);
        }*/
    }
}
