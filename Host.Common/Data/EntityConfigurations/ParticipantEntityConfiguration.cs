using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="DbParticipant"/>.
    /// </summary>
    public class ParticipantEntityConfiguration : IEntityTypeConfiguration<DbParticipant>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DbParticipant> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(quiz => quiz.Id);

            builder
                .HasMany(participant => participant.Answers)
                .WithOne(answer => answer.Participant)
                .HasForeignKey(answer => answer.ParticipantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("QuibbleParticipants");
        }
    }
}