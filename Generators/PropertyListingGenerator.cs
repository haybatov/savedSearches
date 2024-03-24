using static GeneratorData;

public class PropertyListingGenerator
{
    private static Random random = new Random();
    private static List<int> apartmentRoomDistribution = Enumerable.Repeat(1, 40).Concat(Enumerable.Repeat(2, 40)).Concat(Enumerable.Repeat(3, 20)).ToList();


    public static IEnumerable<PropertyListing> GeneratePropertyListings()
    {
        int i = 0;
        while (true)
        {
            var listing = new PropertyListing($"prop{i}", "Sample Property", GetRandomLocation(), "");
            listing.Bedrooms = GetRandomBedrooms(listing.Location, out PropertyType type);
            listing.Type = type;
            listing.Status = i % 2 == 0 ? ListingStatus.ForSale : ListingStatus.ForRent;
            SetPrice(listing);

            i++;
            yield return listing;
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
        if (SydneyCbdSuburbs.Contains(location) || MelbourneCbdSuburbs.Contains(location))
        {
            type = PropertyType.Apartment;
            return apartmentRoomDistribution[random.Next(apartmentRoomDistribution.Count)];
        }
        else
        {
            type = PropertyType.House;
            return random.Next(3, 7); // 3 to 6 bedrooms
        }
    }

    private static void SetPrice(PropertyListing listing)
    {
        if (listing.Status == ListingStatus.ForRent)
        {
            var basePrice = listing.Type == PropertyType.Apartment && (SydneySuburbs.Contains(listing.Location) || MelbourneSuburbs.Contains(listing.Location)) ? 650 : 400;
            listing.Price = $"${basePrice * listing.Bedrooms}";
        }
        else // ForSale
        {
            if (random.NextDouble() < 0.5) // 50% Auction or Contact Agent
            {
                listing.Price = random.NextDouble() < 0.5 ? "Auction" : "Contact Agent";
            }
            else
            {
                var basePrice = listing.Type == PropertyType.House ? 350000 : 250000;
                var fixedAmount = listing.Type == PropertyType.House ? 400000 : 300000;
                listing.NumericPrice = basePrice * listing.Bedrooms ?? 0 + fixedAmount;
                listing.Price = $"${listing.NumericPrice.Value:N0}";
            }
        }
    }

    public static void MainDemo(string[] args)
    {
        var propertyListings = GeneratePropertyListings();

        // Example: Print details of the first 5 generated listings
        foreach (var listing in propertyListings.Take(5))
        {
            Console.WriteLine($"Listing ID: {listing.ListingId}, Location: {listing.Location}, Price: {listing.Price}, Bedrooms: {listing.Bedrooms}, Type: {listing.Type}, Status: {listing.Status}");
        }
    }
}

