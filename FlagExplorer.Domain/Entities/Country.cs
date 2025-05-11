using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagExplorer.Domain.Entities;
public class Country
{
    public string Name { get; set; }
    public string Flag { get; set; }
}

public class CountryDetails : Country
{
    public int Population { get; set; }
    public string Capital { get; set; }
}