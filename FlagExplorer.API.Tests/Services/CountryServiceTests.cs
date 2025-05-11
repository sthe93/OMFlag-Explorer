using FlagExplorer.Application.DTOs;
using FlagExplorer.Infrastructure.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace FlagExplorer.API.Tests.Services
{
    public class CountryServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly CountryService _countryService;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public CountryServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://restcountries.com/v3.1/")
            };
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _countryService = new CountryService(_httpClient);
        }

        [Fact]
        public async Task GetAllCountriesAsync_ReturnsCountries()
        {
            
            var responseJson = @"[
                {
                    ""name"": {""common"": ""USA""},
                    ""flags"": {""png"": ""usa.png""}
                },
                {
                    ""name"": {""common"": ""Canada""},
                    ""flags"": {""png"": ""canada.png""}
                }
            ]";

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("all?fields=name,flags")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            
            var result = await _countryService.GetAllCountriesAsync();

         
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "USA");
            Assert.Contains(result, c => c.Name == "Canada");
        }

        [Fact]
        public async Task GetCountryDetailsAsync_ReturnsCountryDetails()
        {
            
            var responseJson = @"[
                {
                    ""name"": {""common"": ""USA""},
                    ""capital"": [""Washington, D.C.""],
                    ""population"": 331000000,
                    ""flags"": {""png"": ""usa.png""}
                }
            ]";

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("name/USA")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            
            var result = await _countryService.GetCountryDetailsAsync("USA");

           
            Assert.NotNull(result);
            Assert.Equal("USA", result.Name);
            Assert.Equal("Washington, D.C.", result.Capital);
            Assert.Equal(331000000, result.Population);
        }
    }
}