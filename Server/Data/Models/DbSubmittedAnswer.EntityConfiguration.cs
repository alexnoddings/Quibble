using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Server.Data.Models
{
    public class DbSubmittedAnswerEntityConfiguration : DbEntityConfiguration<DbSubmittedAnswer>
    {
        public override void Configure(EntityTypeBuilder<DbSubmittedAnswer> builder)
        {
            base.Configure(builder);

            builder.
        }
    }
}
