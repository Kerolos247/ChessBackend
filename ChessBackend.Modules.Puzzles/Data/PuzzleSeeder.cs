using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ChessBackend.Modules.Puzzles.Entities;

namespace ChessBackend.Modules.Puzzles.Data;

public class LichessPuzzleCsvRow
{
    public string FenBoardState { get; set; } = string.Empty;
    public string SolutionMoves { get; set; } = string.Empty;
}

public sealed class LichessPuzzleCsvMap : ClassMap<LichessPuzzleCsvRow>
{
    public LichessPuzzleCsvMap()
    {
        Map(m => m.FenBoardState).Index(1);
        Map(m => m.SolutionMoves).Index(2);
    }
}

public class PuzzleSeeder
{
    private const string CsvFilePath = @"C:\Users\kero\Downloads\lichess_db_puzzle.csv\lichess_db_puzzle.csv";
    private const int MaxPuzzlesToSeed = 5000;

    private readonly PuzzlesDbContext _dbContext;
    private readonly ILogger<PuzzleSeeder> _logger;

    public PuzzleSeeder(PuzzlesDbContext dbContext, ILogger<PuzzleSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _dbContext.Puzzles.AnyAsync())
        {
            _logger.LogInformation("Puzzles table already contains data. Skipping seeding.");
            return;
        }

        if (!File.Exists(CsvFilePath))
        {
            _logger.LogWarning("Lichess puzzle CSV file not found at {CsvFilePath}. Skipping seeding.", CsvFilePath);
            return;
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
            BadDataFound = null
        };

        var puzzlesToInsert = new List<Puzzle>(MaxPuzzlesToSeed);

        using var reader = new StreamReader(CsvFilePath);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<LichessPuzzleCsvMap>();

        while (await csv.ReadAsync() && puzzlesToInsert.Count < MaxPuzzlesToSeed)
        {
            var row = csv.GetRecord<LichessPuzzleCsvRow>();
            if (row == null || string.IsNullOrWhiteSpace(row.FenBoardState) || string.IsNullOrWhiteSpace(row.SolutionMoves))
                continue;

            if (string.Equals(row.FenBoardState.Trim(), "FEN", StringComparison.OrdinalIgnoreCase))
                continue;

            puzzlesToInsert.Add(new Puzzle
            {
                Id = Guid.NewGuid(),
                FenBoardState = row.FenBoardState.Trim(),
                SolutionMoves = row.SolutionMoves.Trim()
            });
        }

        if (puzzlesToInsert.Count > 0)
        {
            await _dbContext.Puzzles.AddRangeAsync(puzzlesToInsert);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully seeded {Count} puzzles into the database.", puzzlesToInsert.Count);
        }
    }
}
