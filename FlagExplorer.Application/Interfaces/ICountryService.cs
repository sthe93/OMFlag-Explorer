using FlagExplorer.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Application/Interfaces/ICountryService.cs
namespace FlagExplorer.Application.Interfaces;
public interface ICountryService
{
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
    Task<CountryDetailsDto> GetCountryDetailsAsync(string name);
}