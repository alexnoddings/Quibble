using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Data.Entites.Answers;

namespace Quibble.Data.Entites.Questions.Answer;

// [JsonDerivedType(typeof(QuestionAnswerText), typeDiscriminator: "text")]
// [JsonDerivedType(typeof(QuestionAnswerChoice), typeDiscriminator: "choice")]
public /*abstract*/ class QuestionAnswer
{
    public Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;

    // public QuestionAnswerType Type { get; set; }

    public string Answer { get; set; } = string.Empty;

    public List<SubmittedAnswer> SubmittedAnswers { get; init; } = null!;
}

// public enum QuestionAnswerType
// {
//     Text,
//     Choice
// }

public class QuestionAnswerEntityTypeConfiguration : IEntityTypeConfiguration<QuestionAnswer>
{
    public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
    {
        builder.HasKey(qa => qa.QuestionId);

        // builder
        //     .HasDiscriminator(qa => qa.Type)
        //     .HasValue<QuestionAnswerText>(QuestionAnswerType.Text)
        //     .HasValue<QuestionAnswerChoice>(QuestionAnswerType.Choice);

        builder
            .Property(qa => qa.Answer)
            .HasMaxLength(QuestionAnswerTextConstraints.Answer.MaxLength);

        builder
            .HasMany(qa => qa.SubmittedAnswers)
            .WithOne(sa => sa.QuestionAnswer)
            .HasForeignKey(sa => sa.QuestionAnswerId)
            .IsRequired();
    }
}

// public class QuestionAnswerText : QuestionAnswer
// {
//     public string Answer { get; set; } = string.Empty;
//
//     public List<SubmittedAnswerText> SubmittedAnswers { get; set; } = null!;
// }

public static class QuestionAnswerTextConstraints
{
    public static class Answer
    {
        public const int MaxLength = 200;
    }
}

// public class QuestionAnswerTextEntityTypeConfiguration : IEntityTypeConfiguration<QuestionAnswerText>
// {
//     public void Configure(EntityTypeBuilder<QuestionAnswerText> builder)
//     {
//         builder
//             .Property(qat => qat.Answer)
//             .HasMaxLength(QuestionAnswerTextConstraints.Answer_MaxLength);
//     }
// }
//
// public class QuestionAnswerChoice : QuestionAnswer
// {
//     public Guid CorrectOptionId { get; set; }
//     public QuestionAnswerChoiceOption CorrectOption { get; set; } = null!;
//
//     public List<QuestionAnswerChoiceOption> AllOptions { get; set; } = null!;
//
//     public List<SubmittedAnswerChoice> SubmittedAnswers { get; set; } = null!;
// }
//
// public class QuestionAnswerChoiceEntityTypeConfiguration : IEntityTypeConfiguration<QuestionAnswerChoice>
// {
//     public void Configure(EntityTypeBuilder<QuestionAnswerChoice> builder)
//     {
//         builder
//             .HasMany(qac => qac.AllOptions)
//             .WithOne(qaco => qaco.Answer)
//             .HasForeignKey(qaco => qaco.AnswerId)
//             .IsRequired();
//
//         builder
//             .HasOne(qac => qac.CorrectOption)
//             .WithOne()
//             .HasForeignKey<QuestionAnswerChoice>(qac => qac.CorrectOptionId)
//             .IsRequired();
//     }
// }
//
// public class QuestionAnswerChoiceOption : Entity
// {
//     public Guid AnswerId { get; set; }
//     public QuestionAnswerChoice Answer { get; set; } = null!;
//
//     public string Text { get; set; } = string.Empty;
// }
//
// public static class QuestionAnswerChoiceOptionConstraints
// {
//     public const int Text_MaxLength = 400;
// }
//
// public class QuestionAnswerChoiceOptionEntityTypeConfiguration : IEntityTypeConfiguration<QuestionAnswerChoiceOption>
// {
//     public void Configure(EntityTypeBuilder<QuestionAnswerChoiceOption> builder)
//     {
//         builder.IsEntity();
//
//         builder
//             .Property(qaco => qaco.Text)
//             .HasMaxLength(QuestionAnswerChoiceOptionConstraints.Text_MaxLength);
//     }
// }
