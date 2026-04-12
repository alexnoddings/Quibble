using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Data.Entites.Answers;
using Quibble.Data.Entites.Users;

namespace Quibble.Data.Entites.Games;

public class Participant : Entity
{
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;

    public Guid GameId { get; init; }
    public Game Game { get; init; } = null!;

    public List<SubmittedAnswer> SubmittedAnswers { get; init; } = null!;
}

public class ParticipantEntityTypeConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.IsEntity();

        builder
            .HasIndex(p => new { p.UserId, p.GameId })
            .IsUnique();

        builder
            .HasOne(p => p.User)
            .WithMany(u => u.Participations)
            .HasForeignKey(p => p.UserId)
            .IsRequired();

        builder
            .HasOne(p => p.Game)
            .WithMany(q => q.Participants)
            .HasForeignKey(p => p.GameId)
            .IsRequired();

        builder
            .HasMany(p => p.SubmittedAnswers)
            .WithOne(sa => sa.Participant)
            .HasForeignKey(sa => sa.ParticipantId)
            .IsRequired();
    }
}
