using Microsoft.EntityFrameworkCore;
using ChessBackend.Modules.Matches.Entities;

namespace ChessBackend.Modules.Matches.Data;

public class MatchesDbContext : DbContext
{
    public MatchesDbContext(DbContextOptions<MatchesDbContext> options) : base(options)
    {
    }

    public DbSet<Match> Matches => Set<Match>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new MatchConfiguration());
    }
}
