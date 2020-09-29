using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Common.Data
{
    public interface IQuibbleDbContext
    {
        public DbSet<DbParticipant> Participants { get; set; }
        public DbSet<DbQuiz> Quizzes { get; set; }
        public DbSet<DbRound> Rounds { get; set; }
        public DbSet<DbQuestion> Questions { get; set; }
        public DbSet<DbParticipantAnswer> ParticipantAnswers { get; set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
