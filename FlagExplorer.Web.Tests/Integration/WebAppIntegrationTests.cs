using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FlagExplorer.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Xunit;

namespace FlagExplorer.Web.Tests.Integration
{
    public class WebAppIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public WebAppIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                   
                    services.AddHttpClient("CountryApi")
                        .ConfigurePrimaryHttpMessageHandler(() => _mockHttpMessageHandler.Object);
                });
            });
        }

        [Fact]
        public async Task HomePage_ReturnsSuccessAndExpectedContent()
        {
            
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"Name\":\"TestCountry\",\"Flag\":\"test-flag\"}]",
                                               System.Text.Encoding.UTF8,
                                               "application/json")
                });

            var client = _factory.CreateClient();

            
            var response = await client.GetAsync("/");

            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Flag Explorer", content);
            Assert.Contains("TestCountry", content);
        }
        [Fact]
        public async Task DetailsPage_ReturnsSuccessAndExpectedContent_WhenCountryExists()
        {
            // Arrange: mock API to return detailed country info
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{\"Name\":\"TestCountry\",\"Flag\":\"test-flag\",\"Population\":1000000,\"Capital\":\"TestCapital\"}",
                        Encoding.UTF8,
                        "application/json")
                });

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Home/Details/TestCountry");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("TestCountry", content);
            Assert.Contains("TestCapital", content);
        }

        [Fact]
        public async Task DetailsPage_ReturnsNotFound_WhenCountryDoesNotExist()
        {
            // Arrange: mock API to return 404
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Home/Details/NonExistentCountry");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task ErrorPage_ReturnsSuccessAndExpectedContent()
        {
            
            var client = _factory.CreateClient();

         
            var response = await client.GetAsync("/Home/Error");

         
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Error", content);
        }
    }
}