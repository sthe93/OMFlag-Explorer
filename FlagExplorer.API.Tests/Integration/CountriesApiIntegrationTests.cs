// FlagExplorer.API.Tests/Integration/CountriesApiIntegrationTests.cs
using System.Net;
using System.Net.Http.Json;
using FlagExplorer.API;
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
        public async Task GetAllCountries_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/countries");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCountryDetails_ReturnsSuccess_ForValidCountry()
        {
            // Act
            var response = await _client.GetAsync("/api/countries/usa");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetCountryDetails_ReturnsNotFound_ForInvalidCountry()
        {
            // Act
            var response = await _client.GetAsync("/api/countries/invalidcountryname");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}