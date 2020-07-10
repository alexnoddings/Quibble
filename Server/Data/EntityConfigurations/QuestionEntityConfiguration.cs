using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Server.Models.Participants;
using Quibble.Server.Models.Questions;
using Quibble.Server.Models.Rounds;

namespace Quibble.Server.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="Question"/>.
    /// </summary>
    public class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(question => question.Id);

            builder
                .HasOne<Round>()
                .WithMany()
                .HasForeignKey(question => question.RoundId)
                .IsRequired();

            builder
                .HasMany<SubmittedAnswer>()
                .WithOne()
                .HasForeignKey(submittedAnswer => submittedAnswer.QuestionId)
                .IsRequired();

            builder.ToTable("QuizQuestions");
        }
    }
}
