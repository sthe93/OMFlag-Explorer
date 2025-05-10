// FlagExplorer.Web.Tests/Controllers/HomeControllerTests.cs
using FlagExplorer.Web.Controllers;
using FlagExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Net.Http;
using Xunit;

namespace FlagExplorer.Web.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly HomeController _controller;

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
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"name\":\"USA\",\"flag\":\"usa.png\"}]")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

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
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"name\":\"USA\",\"flag\":\"usa.png\",\"capital\":\"Washington, D.C.\",\"population\":331000000}")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

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
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await _controller.Details("Unknown");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}