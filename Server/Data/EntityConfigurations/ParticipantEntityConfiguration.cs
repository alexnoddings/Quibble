using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Participants;
using Quibble.Common.Quizzes;
using Quibble.Common.SubmittedAnswers;

namespace Quibble.Server.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="Participant"/>.
    /// </summary>
    public class ParticipantEntityConfiguration : IEntityTypeConfiguration<Participant>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(participant => participant.Id);

            builder
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(participant => participant.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder
                .HasOne<Quiz>()
                .WithMany()
                .HasForeignKey(participant => participant.QuizId)
                .IsRequired();

            builder
                .HasMany<SubmittedAnswer>()
                .WithOne()
                .HasForeignKey(submittedAnswer => submittedAnswer.ParticipantId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.ToTable("QuizParticipants");
        }
    }
}
