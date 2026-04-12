using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;

namespace Quibble.Data.Entites.Rounds;

public class Round : Entity
{
    public Guid GameId { get; init; }
    public Game Game { get; init; } = null!;

    public int Order { get; set; }
    public RoundState State { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<Question> Questions { get; init; } = null!;
}

public static class RoundConstraints
{
    public static class Title
    {
        public const int MaxLength = 50;
    }

    public static class Description
    {
        public const int MaxLength = 400;
    }
}

public enum RoundState
{
    Hidden,
    Visible
}

public class RoundEntityTypeConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.IsEntity();

        builder
            .Property(r => r.Title)
            .HasMaxLength(RoundConstraints.Title.MaxLength);

        builder
            .Property(r => r.Description)
            .HasMaxLength(RoundConstraints.Description.MaxLength);

        builder
            .HasMany(r => r.Questions)
            .WithOne(q => q.Round)
            .HasForeignKey(q => q.RoundId)
            .IsRequired();
    }
}
