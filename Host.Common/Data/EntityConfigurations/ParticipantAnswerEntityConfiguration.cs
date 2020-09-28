using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="DbParticipantAnswer"/>.
    /// </summary>
    public class ParticipantAnswerEntityConfiguration : IEntityTypeConfiguration<DbParticipantAnswer>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DbParticipantAnswer> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(quiz => quiz.Id);

            builder.ToTable("QuibbleParticipantAnswers");
        }
    }
}