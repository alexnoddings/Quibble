using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="DbQuiz"/>.
    /// </summary>
    public class QuizEntityConfiguration : IEntityTypeConfiguration<DbQuiz>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DbQuiz> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(quiz => quiz.Id);

            builder
                .HasMany(quiz => quiz.Rounds)
                .WithOne(round => round.Quiz)
                .HasForeignKey(round => round.QuizId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(quiz => quiz.Participants)
                .WithOne(participant => participant.Quiz)
                .HasForeignKey(participant => participant.QuizId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("QuibbleQuizzes");
        }
    }
}