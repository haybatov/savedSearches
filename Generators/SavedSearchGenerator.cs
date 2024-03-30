using static GeneratorData;

public class SavedSearchGenerator
{
    private static readonly Random random = new(-2049);
    private static readonly int[] apartmentRoomDistribution = Enumerable.Repeat(1, 40).Concat(Enumerable.Repeat(2, 40)).Concat(Enumerable.Repeat(3, 20)).ToArray();

    public static IEnumerable<SavedSearch> GenerateSavedSearches()
    {
        while (true)
        {
            var location = GetRandomLocation();
            var type = random.Next(2) == 1 ? PropertyType.Apartment : PropertyType.House;
            var maxBedrooms = GetRandomBedrooms(type);
            var listingStatus = (ListingStatus)random.Next((int)ListingStatus.Sold);
            var maxPrice = GetRandomMaxPrice(maxBedrooms, listingStatus);

            var search = new SavedSearch(location)
            {
                MinBedrooms = maxBedrooms - 1,
                MaxBedrooms = maxBedrooms,
                PropertyType = type,
                MinPrice = 0,
                MaxPrice = maxPrice,
                ListingType = listingStatus,
            };

            yield return search;
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

    private static int? GetRandomMaxPrice(int bedrooms, ListingStatus listingStatus) => listingStatus switch
    {
        ListingStatus.ForRent => random.Next(100, 650) * bedrooms,
        ListingStatus.ForSale => bedrooms * 150_000,
        _ => null
    };
}
