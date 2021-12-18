using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Common.Entities;

namespace Quibble.Server.Core.Models;

public class DbSubmittedAnswer : ISubmittedAnswer
{
	public Guid Id { get; set; }

	public Guid QuestionId { get; set; }
	public DbQuestion Question { get; set; } = default!;

	public Guid ParticipantId { get; set; }
	public DbParticipant Participant { get; set; } = default!;

	public string Text { get; set; } = string.Empty;
	public decimal AssignedPoints { get; set; }
}

public class DbSubmittedAnswerEntityConfiguration : DbEntityConfiguration<DbSubmittedAnswer>
{
	public override void Configure(EntityTypeBuilder<DbSubmittedAnswer> builder)
	{
		base.Configure(builder);

		builder
			.HasOne(answer => answer.Question)
			.WithMany(question => question.SubmittedAnswers)
			.HasForeignKey(answer => answer.QuestionId)
			.IsRequired();

		builder
			.HasOne(answer => answer.Participant)
			.WithMany(participant => participant.SubmittedAnswers)
			.HasForeignKey(answer => answer.ParticipantId)
			.IsRequired();

		builder
			.Property(answer => answer.Text)
			.HasMaxLength(200);

		builder
			.Property(answer => answer.AssignedPoints)
			.HasPrecision(4, 2)
			.HasDefaultValue(-1);

		builder.ToTable("SubmittedAnswers");
	}
}
