using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlagExplorer.Web.Tests.Integration
{
    public class WebAppIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WebAppIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                // Configure test services if needed
                builder.ConfigureServices(services =>
                {
                    // Mock any external services here
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });
        }

        [Fact]
        public async Task HomePage_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task DetailsPage_ReturnsSuccessStatusCode_ForValidCountry()
        {
            // Since this hits your actual API, ensure:
            // 1. Your API is running
            // 2. You have test data or mocks in place

            // Act
            var response = await _client.GetAsync("/Home/Details/usa");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }
    }
}