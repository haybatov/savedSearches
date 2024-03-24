using Microsoft.Data.Sqlite;
using Dapper;
using static System.Net.Mime.MediaTypeNames;
using System.Transactions;

Console.WriteLine("Hello");

// GenerateSavedSearches(5_000_000, Console.WriteLine);
var searches = LoadSavedSearches().GroupBy(x => x.Location).ToDictionary(x => x.Key);

Console.WriteLine($"Loaded {searches.Count} saved searches");

// Generate property listings and search through the saved searches to find any matching listings
foreach (var listing in PropertyListingGenerator.GeneratePropertyListings().Take(100))
{
    var matchingSearches = searches[listing.Location]?.Where(search => 
                                        (!search.MinPrice.HasValue || listing.NumericPrice >= search.MinPrice) &&
                                        (!search.MaxPrice.HasValue || listing.NumericPrice <= search.MaxPrice) &&
                                        (!search.MinBedrooms.HasValue || listing.Bedrooms >= search.MinBedrooms) &&
                                        (!search.MaxBedrooms.HasValue || listing.Bedrooms <= search.MaxBedrooms) &&
                                        (!search.Type.HasValue || listing.Type == search.Type) &&
                                        (!search.MinBathrooms.HasValue || listing.Bathrooms >= search.MinBathrooms) &&
                                        (!search.MaxBathrooms.HasValue || listing.Bathrooms <= search.MaxBathrooms) &&
                                        (!search.MinCarspaces.HasValue || listing.Carspaces >= search.MinCarspaces) &&
                                        (!search.MaxCarspaces.HasValue || listing.Carspaces <= search.MaxCarspaces) &&
                                        (!search.MinLandSize.HasValue || listing.LandSize >= search.MinLandSize) &&
                                        (!search.MaxLandSize.HasValue || listing.LandSize <= search.MaxLandSize) &&
                                        (search.SaleMethod == SaleMethod.AllTypes || listing.SaleMethod == search.SaleMethod) &&
                                        (!search.ExcludeUnderContract.HasValue || listing.IsUnderContract != search.ExcludeUnderContract) &&
                                        (search.OutdoorFeatures.Count == 0 || search.OutdoorFeatures.All(listing.Features.Contains)) &&
                                        (search.IndoorFeatures.Count == 0 || search.IndoorFeatures.All(listing.Features.Contains)) &&
                                        (search.ClimateControlAndEnergy.Count == 0 || search.ClimateControlAndEnergy.All(listing.Features.Contains)) &&
                                        (search.AccessibilityFeatures.Count == 0 || search.AccessibilityFeatures.All(listing.Features.Contains)) &&
                                        (string.IsNullOrWhiteSpace(search.Keywords) || listing.Title.Contains(search.Keywords) || (listing.Description?.Contains(search.Keywords) ?? false)));

    Console.WriteLine($"Listing {listing.ListingId} ({listing.Location}) matches {matchingSearches?.Count() ?? 0} saved searches");
}

    IEnumerable<SavedSearchQuery> LoadSavedSearches()
    {
        using var connection = new SqliteConnection("Data Source=saved_searches.db");
        connection.Open();

        return connection.Query<SavedSearchQuery>("SELECT * FROM SavedSearches");
    }

    static void GenerateSavedSearches(int count = 5_000_000, Action<string>? notify = null)
    {
        // Generate some saved searches and save them in a SQLite database
        using var connection = new SqliteConnection("Data Source=saved_searches.db");

        connection.Open();

        connection.Execute(@"CREATE TABLE IF NOT EXISTS SavedSearches(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Location TEXT,
    MinPrice INTEGER,
    MaxPrice INTEGER,
    MinBedrooms INTEGER,
    MaxBedrooms INTEGER,
    Type INTEGER, --Assuming mapping of the PropertyType enum to integers
    IncludeSold INTEGER, -- Stored as 0 (false) or 1 (true)
    IncludeAuction INTEGER, -- Stored as 0 (false) or 1 (true)
    MinBathrooms INTEGER,
    MaxBathrooms INTEGER,
    MinCarspaces INTEGER,
    MaxCarspaces INTEGER,
    MinLandSize INTEGER,
    MaxLandSize INTEGER,
    SaleMethod INTEGER, -- Assuming mapping of the SaleMethod enum to integers
    ExcludeUnderContract INTEGER, -- Stored as 0 (false) or 1 (true)
    OutdoorFeatures TEXT, -- JSON-encoded list of strings
    IndoorFeatures TEXT, -- JSON-encoded list of strings
    ClimateControlAndEnergy TEXT, -- JSON-encoded list of strings
    AccessibilityFeatures TEXT, -- JSON-encoded list of strings
    Keywords TEXT
    );");

        connection.Execute("PRAGMA journal_mode = OFF"); // Disable journaling for faster inserts

        // Get saved searches and store them in the database
        int i = 0;
        int batchSize = 20000;
        int batchCount = count / batchSize;

        foreach (var savedSearch in Enumerable.Range(1, batchCount).Select(x => SavedSearchGenerator.GenerateSavedSearches().Take(batchSize)))
        {
            using var transaction = connection.BeginTransaction();
            connection.Execute("INSERT INTO SavedSearches (Location, MinPrice, MaxPrice, MinBedrooms, MaxBedrooms, Type, IncludeSold, IncludeAuction, MinBathrooms, MaxBathrooms, MinCarspaces, MaxCarspaces, MinLandSize, MaxLandSize, SaleMethod, ExcludeUnderContract, OutdoorFeatures, IndoorFeatures, ClimateControlAndEnergy, AccessibilityFeatures, Keywords) VALUES (@Location, @MinPrice, @MaxPrice, @MinBedrooms, @MaxBedrooms, @Type, @IncludeSold, @IncludeAuction, @MinBathrooms, @MaxBathrooms, @MinCarspaces, @MaxCarspaces, @MinLandSize, @MaxLandSize, @SaleMethod, @ExcludeUnderContract, @OutdoorFeatures, @IndoorFeatures, @ClimateControlAndEnergy, @AccessibilityFeatures, @Keywords);", savedSearch, transaction);
            transaction.Commit();

            if ((i += batchSize) % 100_000 == 0) notify?.Invoke($"Inserted {i} saved searches");
        }

        notify?.Invoke("Done.");
        connection.Close();
    }
