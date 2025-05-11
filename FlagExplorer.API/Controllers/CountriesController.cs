using FlagExplorer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlagExplorer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCountries()
    {
        var countries = await _countryService.GetAllCountriesAsync();
        return Ok(countries);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetCountryDetails(string name)
    {
        var country = await _countryService.GetCountryDetailsAsync(name);
        if (country == null)
        {
            return NotFound();
        }
        return Ok(country);
    }
}