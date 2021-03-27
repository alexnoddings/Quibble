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
                .HasMany(quiz => quiz.Rounds)
                .WithOne(round => round.Quiz)
                .HasForeignKey(round => round.QuizId)
                .IsRequired();

            builder
                .HasMany(quiz => quiz.Participants)
                .WithMany(user => user.ParticipatedIn)
                .UsingEntity(joinBuilder => joinBuilder.ToTable("QuizParticipants"));

            builder.ToTable("Quizzes");
        }
    }
}
