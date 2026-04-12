using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Data.Entites.Questions.Body;

// [JsonDerivedType(typeof(QuestionBodyText), typeDiscriminator: "text")]
// [JsonDerivedType(typeof(QuestionBodyImage), typeDiscriminator: "image")]
public /*abstract*/ class QuestionBody
{
    public Guid QuestionId { get; init; }
    public Question Question { get; init; } = null!;

    // public QuestionBodyType Type { get; set; }

    public string Text { get; set; } = string.Empty;
}

// public enum QuestionBodyType
// {
//     Text,
//     Image
// }

public class QuestionBodyEntityTypeConfiguration : IEntityTypeConfiguration<QuestionBody>
{
    public void Configure(EntityTypeBuilder<QuestionBody> builder)
    {
        builder.HasKey(qb => qb.QuestionId);

        // builder
        //     .HasDiscriminator(qb => qb.Type)
        //     .HasValue<QuestionBodyText>(QuestionBodyType.Text)
        //     .HasValue<QuestionBodyImage>(QuestionBodyType.Image);

        builder
            .Property(qbt => qbt.Text)
            .HasMaxLength(QuestionBodyTextConstraints.Text.MaxLength);
    }
}

// public class QuestionBodyText : QuestionBody
// {
//     public string Text { get; set; } = string.Empty;
// }

public static class QuestionBodyTextConstraints
{
    public static class Text
    {
        public const int MaxLength = 400;
    }
}

// public class QuestionBodyTextEntityTypeConfiguration : IEntityTypeConfiguration<QuestionBodyText>
// {
//     public void Configure(EntityTypeBuilder<QuestionBodyText> builder)
//     {
//         builder
//             .Property(qbt => qbt.Text)
//             .HasMaxLength(QuestionBodyTextConstraints.Text_MaxLength);
//     }
// }
//
// public class QuestionBodyImage : QuestionBody
// {
//     // Should store an etag
//     public Guid Id { get; set; }
// }
