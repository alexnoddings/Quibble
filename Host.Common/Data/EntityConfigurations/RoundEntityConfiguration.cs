using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="DbRound"/>.
    /// </summary>
    public class RoundEntityConfiguration : IEntityTypeConfiguration<DbRound>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DbRound> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(round => round.Id);

            builder
                .HasMany(round => round.Questions)
                .WithOne(question => question.Round)
                .HasForeignKey(question => question.RoundId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("QuibbleRounds");
        }
    }
}