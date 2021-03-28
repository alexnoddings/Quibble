using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models
{
    public class DbParticipantEntityConfiguration : DbEntityConfiguration<DbParticipant>
    {
        public override void Configure(EntityTypeBuilder<DbParticipant> builder)
        {
            base.Configure(builder);

            builder
                .HasOne(participant => participant.Quiz)
                .WithMany(quiz => quiz.Participants)
                .HasForeignKey(participant => participant.QuizId)
                .IsRequired();

            builder
                .HasOne(participant => participant.User)
                .WithMany(user => user.Participations)
                .HasForeignKey(participant => participant.UserId)
                .IsRequired();

            builder
                .HasMany(participant => participant.SubmittedAnswers)
                .WithOne(answer => answer.Participant)
                .HasForeignKey(answer => answer.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.ToTable("Participants");
        }
    }
}
