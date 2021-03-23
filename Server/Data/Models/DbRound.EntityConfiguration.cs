using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models
{
    public class DbRoundEntityConfiguration : DbEntityConfiguration<DbRound>
    {
        public override void Configure(EntityTypeBuilder<DbRound> builder)
        {
            base.Configure(builder);

            builder
                .HasMany(round => round.Questions)
                .WithOne(question => question.Round)
                .HasForeignKey(question => question.RoundId)
                .IsRequired();
        }
    }
}
