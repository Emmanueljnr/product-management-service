using Moq;
using Moq.Protected;
using ProductManagementAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProdManagementAPI.Tests
{
    public class ExternalProductServiceTests
    {
        [Fact]
        public async Task FetchProductsAsync_ReturnsProducts()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var fakeResponseContent = "{\"Data\":[{\"Asin\":\"ASIN1\",\"Title\":\"Test Product\",\"Price\":99.99,\"Image\":{\"Primary\":\"http://example.com/image1.jpg\",\"Variants\":[\"http://example.com/image2.jpg\"]}}]}";
            var fakeResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(fakeResponseContent, Encoding.UTF8, "application/json")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(fakeResponseMessage);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/") // Use the base address of your actual service
            };

            var service = new ExternalProductService(httpClient);

            // Act
            var products = await service.FetchProductsAsync("Test Product");

            // Assert
            Assert.NotNull(products);
            var productList = products.ToList();
            Assert.Single(productList);
            var product = productList[0];
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(99.99m, product.Price);
            Assert.Contains("http://example.com/image1.jpg", product.Image);
        }


    }

}
