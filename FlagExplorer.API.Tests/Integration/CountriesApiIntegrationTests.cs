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
            
            var response = await _client.GetAsync("/api/countries");

            
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GET_CountryDetails_ReturnsSuccessStatusCode()
        {
           
            var response = await _client.GetAsync("/api/countries/usa");

         
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GET_CountryDetails_ReturnsNotFound_ForInvalidCountry()
        {
           
            var response = await _client.GetAsync("/api/countries/invalidcountryname");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}