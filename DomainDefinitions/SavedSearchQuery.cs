public class SavedSearchQuery
{
	public string Location { get; set; }
	public int? MinPrice { get; set; }
	public int? MaxPrice { get; set; }
	public int? MinBedrooms { get; set; }
	public int? MaxBedrooms { get; set; }
	public PropertyType? Type { get; set; }
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
	public List<string> OutdoorFeatures { get; set; } = new List<string>();
	public List<string> IndoorFeatures { get; set; } = new List<string>();
	public List<string> ClimateControlAndEnergy { get; set; } = new List<string>();
	public List<string> AccessibilityFeatures { get; set; } = new List<string>();
	public string? Keywords { get; set; }

	public SavedSearchQuery(string location)
	{
		Location = location;
	}
	
	public SavedSearchQuery()
	{
		Location = "";
	}
}
