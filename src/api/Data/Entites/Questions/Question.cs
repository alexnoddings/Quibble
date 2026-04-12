using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Data.Entites.Questions.Answer;
using Quibble.Data.Entites.Questions.Body;
using Quibble.Data.Entites.Rounds;

namespace Quibble.Data.Entites.Questions;

public class Question : Entity
{
    public Guid RoundId { get; init; }
    public Round Round { get; init; } = null!;

    public int Order { get; set; }
    public QuestionState State { get; set; }
    public decimal Points { get; set; }

    public QuestionBody Body { get; init; } = null!;
    public QuestionAnswer Answer { get; init; } = null!;
}

public enum QuestionState
{
    Hidden,
    InProgress,
    Marking,
    Revealed
}

public static class QuestionConstraints
{
    public static class Points
    {
        public const int Minimum = -10;
        public const int Maximum = 10;

        public static decimal Clamp(decimal points)
        {
            var clamped = Math.Clamp(
                points,
                min: Minimum,
                max: Maximum
            );
            return Math.Round(clamped, 2, MidpointRounding.ToZero);
        }
    }
}

public class QuestionEntityTypeConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.IsEntity();

        builder
            .HasOne(q => q.Body)
            .WithOne(qb => qb.Question)
            .HasForeignKey<QuestionBody>(qb => qb.QuestionId)
            .IsRequired();

        builder
            .HasOne(q => q.Answer)
            .WithOne(qa => qa.Question)
            .HasForeignKey<QuestionAnswer>(qa => qa.QuestionId)
            .IsRequired();
    }
}
