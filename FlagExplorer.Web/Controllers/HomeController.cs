// Controllers/HomeController.cs

using System.Net;
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

    // Controllers/HomeController.cs
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("CountryApi");
            var response = await client.GetAsync("countries");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<CountryViewModel>());
            }

            var countries = await response.Content.ReadFromJsonAsync<List<CountryViewModel>>();
            return View(countries ?? new List<CountryViewModel>());
        }
        catch
        {
            return View(new List<CountryViewModel>());
        }
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = HttpContext?.TraceIdentifier ?? "unknown"
        });
    }

    public async Task<IActionResult> Details(string name)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("CountryApi");
            var response = await client.GetAsync($"countries/{name}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            response.EnsureSuccessStatusCode();

            var country = await response.Content.ReadFromJsonAsync<CountryDetailsViewModel>();
            return View(country);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch
        {
            // Log the error
            return StatusCode(500, "Error contacting the API");
        }
    }

    
}