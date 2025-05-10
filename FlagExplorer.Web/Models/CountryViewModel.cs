namespace FlagExplorer.Web.Models;

public class CountryViewModel
{
    public required string Name { get; set; }
    public required string Flag { get; set; }
}

public class CountryDetailsViewModel : CountryViewModel
{
    public int Population { get; set; }
    public required string Capital { get; set; }
}