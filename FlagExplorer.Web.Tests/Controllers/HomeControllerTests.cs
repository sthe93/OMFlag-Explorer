using FlagExplorer.Web.Controllers;
using FlagExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected; // Add this using directive
using System.Net;
using System.Net.Http;
using System.Threading; // Add for CancellationToken
using System.Threading.Tasks;
using Xunit;

namespace FlagExplorer.Web.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly HomeController _controller;
        private const string BaseAddress = "http://localhost/api/";

        public HomeControllerTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _controller = new HomeController(_mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfCountries()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().StartsWith($"{BaseAddress}countries")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"name\":\"USA\",\"flag\":\"usa.png\"}]")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(client);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CountryViewModel>>(viewResult.ViewData.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithCountryDetails()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().StartsWith($"{BaseAddress}countries/USA")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"name\":\"USA\",\"flag\":\"usa.png\",\"capital\":\"Washington, D.C.\",\"population\":331000000}")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(client);

            // Act
            var result = await _controller.Details("USA");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CountryDetailsViewModel>(viewResult.Model);
            Assert.Equal("USA", model.Name);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenCountryDoesNotExist()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().StartsWith($"{BaseAddress}countries/Unknown")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(client);

            // Act
            var result = await _controller.Details("Unknown");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}