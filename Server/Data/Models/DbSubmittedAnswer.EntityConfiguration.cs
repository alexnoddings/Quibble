using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models
{
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
}
