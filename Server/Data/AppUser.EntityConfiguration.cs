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
                .IsRequired();

            builder
                .HasMany(user => user.SubmittedAnswers)
                .WithOne(submittedAnswer => submittedAnswer.Submitter)
                .HasForeignKey(submittedAnswer => submittedAnswer.SubmitterId)
                .IsRequired();
        }
    }
}
