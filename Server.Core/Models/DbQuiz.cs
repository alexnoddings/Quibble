using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Entities;

namespace Quibble.Server.Core.Models;

public class DbQuiz : IQuiz
{
	public Guid Id { get; set; }

	public Guid OwnerId { get; set; }
	public DbUser Owner { get; set; } = default!;

	public string Title { get; set; } = string.Empty;
	public QuizState State { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? OpenedAt { get; set; }

	public List<DbRound> Rounds { get; set; } = new();
	public List<DbParticipant> Participants { get; set; } = new();
}

public class DbQuizEntityConfiguration : DbEntityConfiguration<DbQuiz>
{
	public override void Configure(EntityTypeBuilder<DbQuiz> builder)
	{
		base.Configure(builder);

		builder
			.HasOne(quiz => quiz.Owner)
			.WithMany(user => user.Quizzes)
			.HasForeignKey(quiz => quiz.OwnerId)
			.IsRequired();

		builder
			.HasMany(quiz => quiz.Rounds)
			.WithOne(round => round.Quiz)
			.HasForeignKey(round => round.QuizId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		builder
			.HasMany(quiz => quiz.Participants)
			.WithOne(participant => participant.Quiz)
			.HasForeignKey(participant => participant.QuizId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		builder
			.Property(question => question.Title)
			.HasMaxLength(100);

		builder.ToTable("Quizzes");
	}
}
