public class PropertyListing
{
	public string ListingId { get; set; }
	public string Title { get; set; }
	public string Location { get; set; }
	public string Price { get; set; } // Price as a string to accommodate various formats
	public decimal? NumericPrice { get; set; } // Optional numeric price
	public int? Bedrooms { get; set; }
	public int? Bathrooms { get; set; }
	public int? Carspaces { get; set; }

	public int? LandSize { get; set; } // In square meters
	public PropertyType? Type { get; set; }
	public ListingStatus? Status { get; set; } // For sale, for rent, sold
	public string? Description { get; set; }
	public List<string> Features { get; set; } = new List<string>();
	public SaleMethod SaleMethod { get; set; }
	public bool? IsUnderContract { get; set; }
	public List<string> OutdoorFeatures { get; set; } = new List<string>();
	public List<string> IndoorFeatures { get; set; } = new List<string>();
	public List<string> ClimateControlAndEnergy { get; set; } = new List<string>();
	public List<string> AccessibilityFeatures { get; set; } = new List<string>();
	public List<string> Images { get; set; } = new List<string>(); // URLs to images of the property

	public PropertyListing(string listingId, string title, string location, string price)
	{
		ListingId = listingId;
		Title = title;
		Location = location;
		Price = price;
	}
}
