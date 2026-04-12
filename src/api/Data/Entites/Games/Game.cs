using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Data.Entites.Rounds;
using Quibble.Data.Entites.Users;

namespace Quibble.Data.Entites.Games;

public class Game : Entity
{
    public string Slug { get; set; } = string.Empty;

    public Guid OwnerId { get; init; }
    public User Owner { get; init; } = null!;

    public GameState State { get; set; }
    public string Title { get; set; } = string.Empty;

    public List<Participant> Participants { get; init; } = null!;
    public List<Round> Rounds { get; init; } = null!;
}

public static class GameConstraints
{
    public static class Slug
    {
        public const int Length = 6;
    }

    public static class Title
    {
        public const int MaxLength = 100;
    }
}

public enum GameState
{
    Draft,
    Open,
    InProgress,
    Completed
}

public class GameEntityTypeConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder
            .IsEntity();

        builder
            .HasIndex(q => q.Slug)
            .IsUnique();

        builder
            .Property(q => q.Slug)
            .HasMaxLength(GameConstraints.Slug.Length)
            .IsFixedLength();

        builder
            .Property(q => q.Title)
            .HasMaxLength(GameConstraints.Title.MaxLength);

        builder
            .HasMany(q => q.Rounds)
            .WithOne(r => r.Game)
            .HasForeignKey(r => r.GameId)
            .IsRequired();

        builder
            .HasMany(q => q.Participants)
            .WithOne(p => p.Game)
            .HasForeignKey(p => p.GameId)
            .IsRequired();
    }
}
