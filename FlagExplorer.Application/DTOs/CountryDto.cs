using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagExplorer.Application.DTOs;
public class CountryDto
{
    public string Name { get; set; }
    public string Flag { get; set; }
}

public class CountryDetailsDto : CountryDto
{
    public int Population { get; set; }
    public string Capital { get; set; }
}