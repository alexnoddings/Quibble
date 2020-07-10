using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Server.Models.Participants;
using Quibble.Server.Models.Quizzes;
using Quibble.Server.Models.Rounds;
using Quibble.Server.Models.Users;

namespace Quibble.Server.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="Quiz"/>.
    /// </summary>
    public class QuizEntityConfiguration : IEntityTypeConfiguration<Quiz>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(quiz => quiz.Id);

            builder
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(quiz => quiz.OwnerId)
                .IsRequired();

            builder
                .HasMany<Round>()
                .WithOne()
                .HasForeignKey(round => round.QuizId)
                .IsRequired();

            builder
                .HasMany<Participant>()
                .WithOne()
                .HasForeignKey(participant => participant.QuizId)
                .IsRequired();

            builder.ToTable("Quizzes");
        }
    }
}
