using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models
{
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

            builder.ToTable("Questions");
        }
    }
}
