using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Entities;

namespace Quibble.Server.Core.Models;

public class DbQuestion : IQuestion
{
	public Guid Id { get; set; }
	public Guid RoundId { get; set; }
	public DbRound Round { get; set; } = default!;

	public string Text { get; set; } = string.Empty;
	public string Answer { get; set; } = string.Empty;
	public decimal Points { get; set; }
	public QuestionState State { get; set; }
	public int Order { get; set; }

	public List<DbSubmittedAnswer> SubmittedAnswers { get; set; } = new();
}

public class DbQuestionEntityConfiguration : DbEntityConfiguration<DbQuestion>
{
	public override void Configure(EntityTypeBuilder<DbQuestion> builder)
	{
		base.Configure(builder);

		builder
			.HasOne(question => question.Round)
			.WithMany(round => round.Questions)
			.HasForeignKey(question => question.RoundId)
			.IsRequired();

		builder
			.HasMany(question => question.SubmittedAnswers)
			.WithOne(submittedAnswer => submittedAnswer.Question)
			.HasForeignKey(submittedAnswer => submittedAnswer.QuestionId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		builder
			.Property(question => question.Text)
			.HasMaxLength(200);

		builder
			.Property(question => question.Answer)
			.HasMaxLength(200);

		builder
			.Property(question => question.Points)
			.HasPrecision(4, 2);

		builder.ToTable("Questions");
	}
}
