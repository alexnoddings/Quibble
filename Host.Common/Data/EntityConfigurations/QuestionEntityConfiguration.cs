using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="DbQuestion"/>.
    /// </summary>
    public class QuestionEntityConfiguration : IEntityTypeConfiguration<DbQuestion>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DbQuestion> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(question => question.Id);

            builder
                .HasMany(question => question.Answers)
                .WithOne(participantAnswer => participantAnswer.Question)
                .HasForeignKey(participantAnswer => participantAnswer.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("QuibbleQuestions");
        }
    }
}