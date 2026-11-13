using Microsoft.EntityFrameworkCore;
using ChessBackend.Modules.Puzzles.Entities;

namespace ChessBackend.Modules.Puzzles.Data;

public class PuzzlesDbContext : DbContext
{
    public PuzzlesDbContext(DbContextOptions<PuzzlesDbContext> options) : base(options)
    {
    }

    public DbSet<Puzzle> Puzzles => Set<Puzzle>();
    public DbSet<UserPuzzleSolved> UserPuzzlesSolved => Set<UserPuzzleSolved>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new PuzzleConfiguration());
        builder.ApplyConfiguration(new UserPuzzleSolvedConfiguration());
    }
}
