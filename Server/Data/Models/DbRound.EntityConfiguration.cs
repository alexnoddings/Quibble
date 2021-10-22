using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models;

public class DbRoundEntityConfiguration : DbEntityConfiguration<DbRound>
{
    public override void Configure(EntityTypeBuilder<DbRound> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(round => round.Quiz)
            .WithMany(quiz => quiz.Rounds)
            .HasForeignKey(round => round.QuizId)
            .IsRequired();

        builder
            .HasMany(round => round.Questions)
            .WithOne(question => question.Round)
            .HasForeignKey(question => question.RoundId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .Property(round => round.Title)
            .HasMaxLength(100);

        builder.ToTable("Rounds");
    }
}
