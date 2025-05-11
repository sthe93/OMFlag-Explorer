using FlagExplorer.Web.Controllers;
using FlagExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq.Protected;
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
        public async Task Index_ReturnsViewWithCountries()
        {
            
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
                    Content = new StringContent("[{\"Name\":\"TestCountry\",\"Flag\":\"test-flag\"}]")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient("CountryApi"))
                .Returns(httpClient);

          
            var result = await _controller.Index();

            
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CountryViewModel>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("TestCountry", model[0].Name);
        }

       
        [Fact]
        public async Task Index_ReturnsEmptyList_WhenApiFails()
        {
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory.Setup(_ => _.CreateClient("CountryApi"))
                .Returns(httpClient);

         
            var result = await _controller.Index();

           
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CountryViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

         
            var result = _controller.Error();

         
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Details_ReturnsViewWithCountry_WhenCountryExists()
        {
            
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
                    Content = new StringContent("{\"Name\":\"TestCountry\",\"Flag\":\"test-flag\",\"Population\":1000000,\"Capital\":\"TestCapital\"}")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient("CountryApi"))
                .Returns(httpClient);

          
            var result = await _controller.Details("TestCountry");

            
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CountryDetailsViewModel>(viewResult.Model);
            Assert.Equal("TestCountry", model.Name);
            Assert.Equal("TestCapital", model.Capital);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenCountryDoesNotExist()
        {
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Not Found")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };

            _mockHttpClientFactory.Setup(_ => _.CreateClient("CountryApi"))
                .Returns(httpClient);

            
            var result = await _controller.Details("NonExistentCountry");

            
            Assert.IsType<NotFoundResult>(result);
        }
      
    }
}