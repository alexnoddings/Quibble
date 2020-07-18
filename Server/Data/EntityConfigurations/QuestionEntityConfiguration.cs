using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Participants;
using Quibble.Common.Questions;
using Quibble.Common.Rounds;
using Quibble.Common.SubmittedAnswers;

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
