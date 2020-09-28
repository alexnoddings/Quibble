using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="DbQuibbleUser"/>.
    /// </summary>
    public class UserEntityConfiguration : IEntityTypeConfiguration<DbQuibbleUser>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DbQuibbleUser> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            // Default behaviour (e.g. Id) is handled by AspNet

            builder
                .HasMany(user => user.Quizzes)
                .WithOne(quiz => quiz.Owner)
                .HasForeignKey(quiz => quiz.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(user => user.Participants)
                .WithOne(participant => participant.User)
                .HasForeignKey(participant => participant.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("QuibbleUsers");
        }
    }
}