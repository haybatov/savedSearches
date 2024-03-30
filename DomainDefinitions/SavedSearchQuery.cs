public class SavedSearch
{
	public string Location { get; set; }
	public decimal? MinPrice { get; set; }
	public decimal? MaxPrice { get; set; }
	public int? MinBedrooms { get; set; }
	public int? MaxBedrooms { get; set; }
	public PropertyType? PropertyType { get; set; }
	public ListingStatus? ListingType { get; set; }
	public bool? IncludeSold { get; set; }
	public bool? IncludeAuction { get; set; }
	public int? MinBathrooms { get; set; }
	public int? MaxBathrooms { get; set; }
	public int? MinCarspaces { get; set; }
	public int? MaxCarspaces { get; set; }
	public int? MinLandSize { get; set; }
	public int? MaxLandSize { get; set; }
	public SaleMethod SaleMethod { get; set; } = SaleMethod.AllTypes;
	public bool? ExcludeUnderContract { get; set; }
	public List<string> OutdoorFeatures { get; set; } = [];
	public List<string> IndoorFeatures { get; set; } = [];
	public List<string> ClimateControlAndEnergy { get; set; } = [];
	public List<string> AccessibilityFeatures { get; set; } = [];
	public string? Keywords { get; set; }

	public SavedSearch(string location)
	{
		Location = location;
	}
	
	public SavedSearch()
	{
		Location = "";
	}

	public override string ToString()
	{
        return $"Search in {Location} for {MinBedrooms}-{MaxBedrooms} bedrooms, {MinBathrooms}-{MaxBathrooms} bathrooms, {MinCarspaces}-{MaxCarspaces} carspaces, and a price between {MinPrice} and {MaxPrice} for {ListingType}";
    }
}
