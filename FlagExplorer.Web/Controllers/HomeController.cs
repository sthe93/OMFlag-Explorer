// Controllers/HomeController.cs
using FlagExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace FlagExplorer.Web.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("CountryApi");
        var countries = await client.GetFromJsonAsync<List<CountryViewModel>>("countries");

        return View(countries ?? new List<CountryViewModel>());
    }

    public async Task<IActionResult> Details(string name)
    {
        var client = _httpClientFactory.CreateClient("CountryApi");
        var country = await client.GetFromJsonAsync<CountryDetailsViewModel>($"countries/{name}");

        if (country == null)
        {
            return NotFound();
        }

        return View(country);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}