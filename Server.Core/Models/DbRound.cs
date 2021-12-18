using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Entities;

namespace Quibble.Server.Core.Models;

public class DbRound : IRound
{
	public Guid Id { get; set; }

	public Guid QuizId { get; set; }
	public DbQuiz Quiz { get; set; } = default!;

	public string Title { get; set; } = string.Empty;
	public RoundState State { get; set; }
	public int Order { get; set; }

	public List<DbQuestion> Questions { get; set; } = new();
}

public class DbRoundEntityConfiguration : DbEntityConfiguration<DbRound>
{
	public override void Configure(EntityTypeBuilder<DbRound> builder)
	{
		base.Configure(builder);

		builder
			.HasOne(round => round.Quiz)
			.WithMany(quiz => quiz.Rounds)
			.HasForeignKey(round => round.QuizId)
			.IsRequired();

		builder
			.HasMany(round => round.Questions)
			.WithOne(question => question.Round)
			.HasForeignKey(question => question.RoundId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		builder
			.Property(round => round.Title)
			.HasMaxLength(100);

		builder.ToTable("Rounds");
	}
}
