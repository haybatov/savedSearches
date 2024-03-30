using static GeneratorData;

public class PropertyListingGenerator
{
    private static readonly Random random = new(-1736);
    private static readonly int[] apartmentRoomDistribution = Enumerable.Repeat(1, 40).Concat(Enumerable.Repeat(2, 40)).Concat(Enumerable.Repeat(3, 20)).ToArray();


    public static IEnumerable<PropertyListing> GeneratePropertyListings()
    {
        int i = 0;
        while (true)
        {
            var listing = new PropertyListing($"prop{i}", "Sample Property", GetRandomLocation(), "");
           
            var type = random.Next(2) == 0 ? PropertyType.Apartment : PropertyType.House;
            listing.PropertyType = type;
            listing.Bedrooms = GetRandomBedrooms(type);
            listing.ListingType = i % 2 == 0 ? ListingStatus.ForSale : ListingStatus.ForRent;
            listing.ID = i;
            listing.NumericPrice = GetPrice(listing);

            i++;
            yield return listing;
        }
    }

    private static string GetRandomLocation() => random.NextDouble() switch
    {
        < 0.34 => SydneySuburbs[random.Next(SydneySuburbs.Length)],
        < 0.67 => MelbourneSuburbs[random.Next(MelbourneSuburbs.Length)],
        _ => OtherLocations[random.Next(OtherLocations.Length)]
    };

    private static int GetRandomBedrooms(PropertyType type) => type switch
    {
        PropertyType.Apartment => apartmentRoomDistribution[random.Next(apartmentRoomDistribution.Length)],
        _ => random.Next(1, 7)
    };

    private static int? GetPrice(PropertyListing listing) => listing.ListingType switch
    {
        ListingStatus.ForRent => random.Next(100, 800) * listing.Bedrooms,
        _ => random.Next(20_000, 700_000) * listing.Bedrooms
    };


}

