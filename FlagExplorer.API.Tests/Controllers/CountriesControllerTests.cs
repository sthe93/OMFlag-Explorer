// FlagExplorer.API.Tests/Controllers/CountriesControllerTests.cs
using FlagExplorer.API.Controllers;
using FlagExplorer.Application.DTOs;
using FlagExplorer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlagExplorer.API.Tests.Controllers
{
    public class CountriesControllerTests
    {
        private readonly Mock<ICountryService> _mockService;
        private readonly CountriesController _controller;

        public CountriesControllerTests()
        {
            _mockService = new Mock<ICountryService>();
            _controller = new CountriesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllCountries_ReturnsOkResult_WithListOfCountries()
        {
            // Arrange
            var countries = new List<CountryDto>
            {
                new() { Name = "USA", Flag = "usa.png" },
                new() { Name = "Canada", Flag = "canada.png" }
            };

            _mockService.Setup(s => s.GetAllCountriesAsync()).ReturnsAsync(countries);

            // Act
            var result = await _controller.GetAllCountries();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CountryDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetCountryDetails_ReturnsOkResult_WhenCountryExists()
        {
            // Arrange
            var country = new CountryDetailsDto
            {
                Name = "USA",
                Flag = "usa.png",
                Capital = "Washington, D.C.",
                Population = 331000000
            };

            _mockService.Setup(s => s.GetCountryDetailsAsync("USA")).ReturnsAsync(country);

            // Act
            var result = await _controller.GetCountryDetails("USA");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CountryDetailsDto>(okResult.Value);
            Assert.Equal("USA", returnValue.Name);
        }

        [Fact]
        public async Task GetCountryDetails_ReturnsNotFound_WhenCountryDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetCountryDetailsAsync(It.IsAny<string>()))
                .ReturnsAsync((CountryDetailsDto)null);

            // Act
            var result = await _controller.GetCountryDetails("Unknown");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}