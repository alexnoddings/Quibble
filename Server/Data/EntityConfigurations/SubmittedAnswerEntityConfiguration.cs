using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Participants;
using Quibble.Common.Questions;
using Quibble.Common.SubmittedAnswers;

namespace Quibble.Server.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="SubmittedAnswer"/>.
    /// </summary>
    public class SubmittedAnswerEntityConfiguration : IEntityTypeConfiguration<SubmittedAnswer>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<SubmittedAnswer> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(submittedAnswer => submittedAnswer.Id);

            builder
                .HasOne<Participant>()
                .WithMany()
                .HasForeignKey(submittedAnswer => submittedAnswer.ParticipantId)
                .IsRequired();

            builder
                .HasOne<Question>()
                .WithMany()
                .HasForeignKey(submittedAnswer => submittedAnswer.QuestionId)
                .IsRequired();

            builder.ToTable("QuizSubmittedAnswers");
        }
    }
}
