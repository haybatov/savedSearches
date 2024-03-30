using MatchingSearchResult = (int ListingId, string Location, int Count);
using System.Diagnostics;
using System.Collections.Concurrent;

const int noOfSavedSearches = 5_000_000;

Console.WriteLine($"Loading {noOfSavedSearches:N0} test saved searches...");

Stopwatch stopwatch = Stopwatch.StartNew();

#if Sqlite_test
// Next lines are used for estimating the performance of Sqlite database for storing and retrieving saved searches.
// Uncomment the next line to generate and store saved searches in a Sqlite database on the first run.
// GenerateSavedSearches(noOfSavedSearches, Console.WriteLine);
var searches = LoadSavedSearches().GroupBy(x => x.Location).ToDictionary(x => x.Key);
#else
var searches = SavedSearchGenerator.GenerateSavedSearches().Take(noOfSavedSearches).GroupBy(x => x.Location).ToDictionary(x => x.Key);
#endif

Console.WriteLine($"Loaded {noOfSavedSearches:N0} saved searches for {searches.Count} locations in {stopwatch.ElapsedMilliseconds} ms.");

PropertyListing[] propertyListings = PropertyListingGenerator.GeneratePropertyListings().Take(5000).ToArray();

ConcurrentBag<MatchingSearchResult> matchingSearchResults = [];
stopwatch.Restart();


// Match listings to saved searches
Parallel.For(0, propertyListings.Length, i =>
{
    var listing = propertyListings[i];

    var matchingSearches = searches[listing.Location]?.Where(search =>
                                        (!search.MinPrice.HasValue || listing.NumericPrice >= search.MinPrice) &&
                                        (!search.MaxPrice.HasValue || listing.NumericPrice <= search.MaxPrice) &&
                                        (!search.MinBedrooms.HasValue || listing.Bedrooms >= search.MinBedrooms) &&
                                        (!search.MaxBedrooms.HasValue || listing.Bedrooms <= search.MaxBedrooms) &&
                                        (!search.ListingType.HasValue || listing.ListingType == search.ListingType) &&
                                        (!search.PropertyType.HasValue || listing.PropertyType == search.PropertyType) &&
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

    matchingSearchResults.Add((listing.ID, listing.Location, matchingSearches?.Count() ?? 0));
});

Console.WriteLine($"Processed all listings in {stopwatch.ElapsedMilliseconds} ms.");

var orderedResults = matchingSearchResults.OrderByDescending(x => x.Count);

// We want to show the top 10 apartments and top 10 houses that match the most saved searches
// which does not happen naturally because houses have wider spread of bedrooms, therefore, they match less searches
// and apartments dominate the top of the list by a substantial margin.

var resultsToShow = orderedResults.Where(x => propertyListings[x.ListingId].PropertyType == PropertyType.Apartment).Take(10)
    .Concat(orderedResults.Where(x => propertyListings[x.ListingId].PropertyType == PropertyType.House).Take(10));

resultsToShow
    .ToList()
    .ForEach(x => Console.WriteLine($"Listing {propertyListings[x.ListingId]} matched {x.Count:N0} saved searches."));

// Interested in the distribution of searches by suburbs? Uncomment the next line
// matchingSearchResults.GroupBy(x=>x.Location).Select(x=>(Key: x.Key, Count: x.Count())).OrderByDescending(x=>x.Count).ToList().ForEach(x=>Console.WriteLine($"Location {x.Key} matched {x.Count} searches"));
