using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quibble.Host.Common;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted.Platform.Services
{
    public class HostedRoundService : HostedServiceBase, IRoundService
    {
        private IRoundEventsInvoker RoundEvents { get; }

        public HostedRoundService(IServiceProvider serviceProvider, IRoundEventsInvoker roundEventsInvoker)
            : base(serviceProvider)
        {
            RoundEvents = roundEventsInvoker;
        }

        public async Task CreateAsync(Guid parentQuizId)
        {
            DbQuiz? quiz = await DbContext.Quizzes.FindAsync(parentQuizId);
            if (quiz == null)
                throw ThrowHelper.NotFound("Quiz", parentQuizId);

            DbQuibbleUser user = await GetCurrentUserAsync();
            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised("You are not the quiz owner.");

            var round = new DbRound {QuizId = quiz.Id};
            DbContext.Rounds.Add(round);
            await DbContext.SaveChangesAsync();
            await RoundEvents.InvokeRoundAddedAsync(round);
        }

        public async Task<List<DtoRound>> GetForQuizAsync(Guid quizId)
        {
            DbQuiz? quiz = await DbContext.Quizzes.FindAsync(quizId);
            if (quiz == null)
                throw ThrowHelper.NotFound("Quiz", quizId);

            var rounds =
                from round in DbContext.Rounds
                where round.QuizId == quizId
                select new DtoRound(round);
            return await rounds.ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            DbRound? round = await DbContext.Rounds.FindAsync(id);
            if (round == null)
                throw ThrowHelper.NotFound("Round", id);

            Guid quizOwnerId = await DbContext.Quizzes.Where(q => q.Id == round.QuizId).Select(q => q.OwnerId).FirstAsync();
            DbQuibbleUser user = await GetCurrentUserAsync();
            if (user.Id != quizOwnerId)
                throw ThrowHelper.Unauthorised("You are not the quiz owner.");

            DbContext.Rounds.Remove(round);
            await DbContext.SaveChangesAsync();
            await RoundEvents.InvokeRoundDeletedAsync(id);
        }

        public async Task UpdateTitleAsync(Guid id, string newTitle)
        {
            newTitle ??= string.Empty;

            DbRound? round = await DbContext.Rounds.FindAsync(id);
            if (round == null)
                throw ThrowHelper.NotFound("Round", id);

            DbQuiz quiz = await DbContext.Quizzes.FindAsync(round.QuizId);
            DbQuibbleUser user = await GetCurrentUserAsync();
            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised("You are not the quiz owner.");
            if (quiz.IsPublished)
                throw ThrowHelper.InvalidOperation("Cannot edit a published quiz.");

            round.Title = newTitle;
            await DbContext.SaveChangesAsync();
            await RoundEvents.InvokeTitleUpdatedAsync(id, newTitle);
        }
    }
}
