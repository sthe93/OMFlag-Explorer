// Infrastructure/Services/CountryService.cs

using System.Net;
using FlagExplorer.Application.Interfaces;
using FlagExplorer.Application.DTOs;
using System.Text.Json;

namespace FlagExplorer.Infrastructure.Services;

public class CountryService : ICountryService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public CountryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://restcountries.com/v3.1/");
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
    {
        var response = await _httpClient.GetAsync("all?fields=name,flags");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var countries = JsonSerializer.Deserialize<List<RestCountry>>(content, _options);

        return countries.Select(c => new CountryDto
        {
            Name = c.Name.Common,
            Flag = c.Flags.Png
        });
    }

    public async Task<CountryDetailsDto?> GetCountryDetailsAsync(string name)
    {
        try
        {
            var response = await _httpClient.GetAsync($"name/{name}?fields=name,capital,population,flags");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var country = JsonSerializer.Deserialize<List<RestCountry>>(content, _options)?.FirstOrDefault();

            if (country == null)
            {
                return null;
            }

            return new CountryDetailsDto
            {
                Name = country.Name.Common,
                Flag = country.Flags.Png,
                Capital = country.Capital?.FirstOrDefault(),
                Population = country.Population
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    private class RestCountry
    {
        public Name Name { get; set; }
        public List<string> Capital { get; set; }
        public int Population { get; set; }
        public Flags Flags { get; set; }
    }

    private class Name
    {
        public string Common { get; set; }
    }

    private class Flags
    {
        public string Png { get; set; }
    }
}