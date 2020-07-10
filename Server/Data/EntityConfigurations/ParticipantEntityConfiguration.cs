using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Server.Models.Participants;
using Quibble.Server.Models.Quizzes;
using Quibble.Server.Models.Users;

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
