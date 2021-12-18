using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Entities;

namespace Quibble.Server.Core.Models;

public class DbUser : IdentityUser<Guid>, IEntity
{
	public static Guid DeletedUserId { get; } = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

	public List<DbQuiz> Quizzes { get; set; } = new();
	public List<DbParticipant> Participations { get; set; } = new();
}

public class AppUserEntityConfiguration : DbEntityConfiguration<DbUser>
{
	public override void Configure(EntityTypeBuilder<DbUser> builder)
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

		builder.HasData(new DbUser
		{
			Id = DbUser.DeletedUserId,
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
