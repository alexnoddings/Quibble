using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Questions.Answer;

namespace Quibble.Data.Entites.Answers;

public /*abstract*/ class SubmittedAnswer
{
    public QuestionAnswer QuestionAnswer { get; init; } = null!;
    public Guid QuestionAnswerId { get; init; }

    public Guid ParticipantId { get; init; }
    public Participant Participant { get; init; } = null!;

    public decimal? Points { get; set; }
    public string Answer { get; set; } = string.Empty;
}

public class SubmittedAnswerEntityTypeConfiguration : IEntityTypeConfiguration<SubmittedAnswer>
{
    public void Configure(EntityTypeBuilder<SubmittedAnswer> builder)
    {
        builder.HasKey(sa => new { sa.QuestionAnswerId, sa.ParticipantId });

        // builder
        //     .HasDiscriminator<int>("Type")
        //     .HasValue<SubmittedAnswerText>(0)
        //     .HasValue<SubmittedAnswerChoice>(1);

        builder
            .HasOne(sa => sa.Participant)
            .WithMany(p => p.SubmittedAnswers)
            .HasForeignKey(sa => sa.ParticipantId)
            .IsRequired();

        builder
            .HasOne(sa => sa.Participant)
            .WithMany(p => p.SubmittedAnswers)
            .HasForeignKey(sa => sa.ParticipantId)
            .IsRequired();

        builder
            .Property(sat => sat.Answer)
            .HasMaxLength(SubmittedAnswerConstraints.Answer.MaxLength);

        builder
            .ToTable("SubmittedAnswers");
    }
}

// public class SubmittedAnswerText : SubmittedAnswer
// {
//     public QuestionAnswerText QuestionAnswer { get; set; } = null!;
//
//     public string Answer { get; set; } = string.Empty;
// }

public static class SubmittedAnswerConstraints
{
    public static class Answer
    {
        public const int MaxLength = 200;
    }

    public static class Points
    {
        public const int Minimum = QuestionConstraints.Points.Minimum;
        public const int Maximum = QuestionConstraints.Points.Maximum;
    }
}

// public class SubmittedAnswerTextEntityTypeConfiguration : IEntityTypeConfiguration<SubmittedAnswerText>
// {
//     public void Configure(EntityTypeBuilder<SubmittedAnswerText> builder)
//     {
//         builder
//             .HasOne(sat => sat.QuestionAnswer)
//             .WithMany(qat => qat.SubmittedAnswers)
//             .HasForeignKey(sat => sat.QuestionAnswerId)
//             .IsRequired();
//
//         builder
//             .Property(sat => sat.Answer)
//             .HasMaxLength(SubmittedAnswerTextConstraints.Answer_MaxLength);
//     }
// }
//
// public class SubmittedAnswerChoice : SubmittedAnswer
// {
//     public QuestionAnswerChoice QuestionAnswer { get; set; } = null!;
//
//     public Guid SelectedId { get; set; }
//     public QuestionAnswerChoiceOption Selected { get; set; } = null!;
// }
//
// public class SubmittedAnswerChoiceEntityTypeConfiguration : IEntityTypeConfiguration<SubmittedAnswerChoice>
// {
//     public void Configure(EntityTypeBuilder<SubmittedAnswerChoice> builder)
//     {
//         builder
//             .HasOne(sac => sac.QuestionAnswer)
//             .WithMany(qac => qac.SubmittedAnswers)
//             .HasForeignKey(sac => sac.QuestionAnswerId)
//             .IsRequired();
//
//         builder
//             .HasOne(sac => sac.Selected)
//             .WithMany()
//             .HasForeignKey(sac => sac.SelectedId)
//             .IsRequired();
//     }
// }
