using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Data.Identity;

namespace Quibble.Host.Common.Data
{
    public class QuibbleDbContext : IdentityDbContext<DbQuibbleUser, QuibbleIdentityRole, Guid>, IQuibbleDbContext
    {
        public DbSet<DbParticipant> Participants { get; set; } = default!;
        public DbSet<DbQuiz> Quizzes { get; set; } = default!;
        public DbSet<DbRound> Rounds { get; set; } = default!;
        public DbSet<DbQuestion> Questions { get; set; } = default!;
        public DbSet<DbParticipantAnswer> ParticipantAnswers { get; set; } = default!;

        public QuibbleDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(QuibbleDbContext).Assembly);
        }
    }
}