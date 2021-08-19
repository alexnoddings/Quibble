using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Server.Data.Models;

namespace Quibble.Server.Data
{
    public class AppUserEntityConfiguration : DbEntityConfiguration<AppUser>
    {
        public override void Configure(EntityTypeBuilder<AppUser> builder)
        {
            base.Configure(builder);

            builder
                .HasMany(user => user.Quizzes)
                .WithOne(quiz => quiz.Owner)
                .HasForeignKey(quiz => quiz.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasMany(user => user.Participations)
                .WithOne(participant => participant.User)
                .HasForeignKey(participant => participant.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasData(new AppUser
            {
                Id = AppUser.DeletedUserId,
                UserName = "[Deleted User]",
                NormalizedUserName = "[DELETED USER]",
                Email = string.Empty,
                NormalizedEmail = string.Empty,
                PasswordHash = string.Empty,
                PhoneNumber = string.Empty,
                SecurityStamp = string.Empty,
                ConcurrencyStamp = string.Empty,
                LockoutEnabled = true,
                LockoutEnd = DateTimeOffset.MaxValue
            });
        }
    }
}
