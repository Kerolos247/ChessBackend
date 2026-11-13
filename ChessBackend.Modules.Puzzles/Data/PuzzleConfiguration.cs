using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ChessBackend.Modules.Puzzles.Entities;

namespace ChessBackend.Modules.Puzzles.Data;

public class PuzzleConfiguration : IEntityTypeConfiguration<Puzzle>
{
    public void Configure(EntityTypeBuilder<Puzzle> builder)
    {
        builder.ToTable("Puzzles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FenBoardState)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(p => p.SolutionMoves)
            .HasMaxLength(500)
            .IsRequired();
    }
}

public class UserPuzzleSolvedConfiguration : IEntityTypeConfiguration<UserPuzzleSolved>
{
    public void Configure(EntityTypeBuilder<UserPuzzleSolved> builder)
    {
        builder.ToTable("UserPuzzlesSolved");

        builder.HasKey(ups => new { ups.UserId, ups.PuzzleId });

        builder.HasOne(ups => ups.Puzzle)
            .WithMany(p => p.UserPuzzlesSolved)
            .HasForeignKey(ups => ups.PuzzleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ups => ups.UserId);
    }
}
