using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlagExplorer.API.Tests.Integration
{
    public class CountriesApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CountriesApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GET_Countries_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/countries");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GET_CountryDetails_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/countries/usa");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GET_CountryDetails_ReturnsNotFound_ForInvalidCountry()
        {
            // Act
            var response = await _client.GetAsync("/api/countries/invalidcountryname");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}