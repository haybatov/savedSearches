using static GeneratorData;

public class SavedSearchGenerator
{
    private static Random random = new Random();
    private static List<int> apartmentRoomDistribution = Enumerable.Repeat(1, 40).Concat(Enumerable.Repeat(2, 40)).Concat(Enumerable.Repeat(3, 20)).ToList();

    public static IEnumerable<SavedSearchQuery> GenerateSavedSearches()
    {
        while(true)
        {
            var location = GetRandomLocation();
            var maxBedrooms = GetRandomBedrooms(location, out PropertyType type);
            var maxPrice = GetRandomMaxPrice(maxBedrooms, location, type);

            var search = new SavedSearchQuery(location)
            {
                MinBedrooms= maxBedrooms - 1,
                MaxBedrooms = maxBedrooms,
                Type = type,
                MaxPrice = maxPrice
            };

            yield return search;
        }
    }

    private static string GetRandomLocation()
    {
        if (random.NextDouble() < 0.8)
        {
            return random.NextDouble() < 0.5 ? SydneySuburbs[random.Next(SydneySuburbs.Count)] : MelbourneSuburbs[random.Next(MelbourneSuburbs.Count)];
        }
        else
        {
            return OtherLocations[random.Next(OtherLocations.Count)];
        }
    }

    private static int GetRandomBedrooms(string location, out PropertyType type)
    {
        // Adjust logic based on property type preferences
        type = PropertyType.House;
        if (SydneySuburbs.Contains(location) || MelbourneSuburbs.Contains(location))
        {
            type = PropertyType.Apartment;
            return apartmentRoomDistribution[random.Next(apartmentRoomDistribution.Count)];
        }
        else
        {
            return random.Next(3, 7); // 3 to 6 bedrooms for houses
        }
    }

    private static int? GetRandomMaxPrice(int bedrooms, string location, PropertyType type)
    {
        // Adjust pricing logic to reflect similar distribution to property listings
        if (type == PropertyType.Apartment && (SydneySuburbs.Contains(location) || MelbourneSuburbs.Contains(location)))
        {
            return bedrooms * 650000; // Simulating high price per bedroom for CBD apartments
        }
        else
        {
            return (type == PropertyType.House ? 350000 : 250000) * bedrooms + (type == PropertyType.House ? 400000 : 300000);
        }
    }
}
