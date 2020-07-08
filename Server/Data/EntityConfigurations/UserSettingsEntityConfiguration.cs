using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Server.Models.Users;

namespace Quibble.Server.Data.EntityConfigurations
{
    /// <summary>
    /// Provides entity configuration for <see cref="UserSettings"/>.
    /// </summary>
    public class UserSettingsEntityConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.HasKey(userSettings => userSettings.Id);

            builder
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(userSettings => userSettings.UserId)
                .IsRequired();

            builder.ToTable("AspNetUserSettings");
        }
    }
}
