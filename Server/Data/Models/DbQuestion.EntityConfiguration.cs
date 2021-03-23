using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models
{
    public class DbQuestionEntityConfiguration : DbEntityConfiguration<DbQuestion>
    {
        public override void Configure(EntityTypeBuilder<DbQuestion> builder)
        {
            base.Configure(builder);

            builder
                .HasMany(question => question.SubmittedAnswers)
                .WithOne(submittedAnswer => submittedAnswer.Question)
                .HasForeignKey(submittedAnswer => submittedAnswer.QuestionId)
                .IsRequired();
        }
    }
}
