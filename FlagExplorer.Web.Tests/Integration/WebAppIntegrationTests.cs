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
                    // Configure a test HttpClient that uses our mock handler
                    services.AddHttpClient("CountryApi")
                        .ConfigurePrimaryHttpMessageHandler(() => _mockHttpMessageHandler.Object);
                });
            });
        }

        [Fact]
        public async Task HomePage_ReturnsSuccessAndExpectedContent()
        {
            // Arrange
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

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Flag Explorer", content);
            Assert.Contains("TestCountry", content);
        }
        //[Fact]
        //public async Task DetailsPage_ReturnsSuccessAndExpectedContent_WhenCountryExists()
        //{
        //    // Arrange
        //    _mockHttpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.Is<HttpRequestMessage>(req =>
        //                req.RequestUri != null &&
        //                req.RequestUri.PathAndQuery.Contains("/api/countries/TestCountry")),
        //            ItExpr.IsAny<CancellationToken>()
        //        )
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Content = new StringContent(
        //                "{\"Name\":\"TestCountry\",\"Flag\":\"test-flag\",\"Population\":1000000,\"Capital\":\"TestCapital\"}",
        //                Encoding.UTF8,
        //                "application/json")
        //        });

        //    var client = _factory.CreateClient();

        //    // Act
        //    var response = await client.GetAsync("/Home/Details/TestCountry");

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    var content = await response.Content.ReadAsStringAsync();
        //    Assert.Contains("TestCountry", content);
        //    Assert.Contains("TestCapital", content);
        //    Assert.Contains("1,000,000", content);
        //}

        //[Fact]
        //public async Task DetailsPage_ReturnsNotFound_WhenCountryDoesNotExist()
        //{
        //    // Arrange
        //    _mockHttpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.Is<HttpRequestMessage>(req =>
        //                req.RequestUri != null &&
        //                req.RequestUri.PathAndQuery.Contains("/api/countries/NonExistentCountry")),
        //            ItExpr.IsAny<CancellationToken>()
        //        )
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.NotFound
        //        });

        //    var client = _factory.CreateClient();

        //    // Act
        //    var response = await client.GetAsync("/Home/Details/NonExistentCountry");

        //    // Assert
        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        [Fact]
        public async Task ErrorPage_ReturnsSuccessAndExpectedContent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Home/Error");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Error", content);
        }
    }
}