using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models
{
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
}
