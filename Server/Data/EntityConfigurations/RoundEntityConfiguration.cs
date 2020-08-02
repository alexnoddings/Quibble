using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Questions;
using Quibble.Common.Quizzes;
using Quibble.Common.Rounds;

namespace Quibble.Server.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="Round"/>.
    /// </summary>
    public class RoundEntityConfiguration : IEntityTypeConfiguration<Round>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Round> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(round => round.Id);

            builder
                .HasOne<Quiz>()
                .WithMany()
                .HasForeignKey(round => round.QuizId)
                .IsRequired();

            builder
                .HasMany<Question>()
                .WithOne()
                .HasForeignKey(question => question.RoundId)
                .IsRequired();

            builder.ToTable("QuizRounds");
        }
    }
}
