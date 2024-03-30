using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace savedSearches
{
    internal class SqliteRepo
    {
        public static IEnumerable<SavedSearch> LoadSavedSearches()
        {
            using var connection = new SqliteConnection("Data Source=saved_searches.db");
            connection.Open();

            return connection.Query<SavedSearch>("SELECT * FROM SavedSearches");
        }

        public static void GenerateSavedSearches(int count, Action<string>? notify = null)
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

    }
}
