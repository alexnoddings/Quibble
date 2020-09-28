using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Events;

namespace Quibble.UI.Core.Services
{
    public class SynchronisedQuizFactory : ISynchronisedQuizFactory
    {
        private IQuizService QuizService { get; }
        private IRoundService RoundService { get; }
        private IQuestionService QuestionService { get; }

        private IQuizEvents QuizEvents { get; }
        private IRoundEvents RoundEvents { get; }
        private IQuestionEvents QuestionEvents { get; }

        public SynchronisedQuizFactory(IServiceProvider serviceProvider)
        {
            QuizService = serviceProvider.GetRequiredService<IQuizService>();
            RoundService = serviceProvider.GetRequiredService<IRoundService>();
            QuestionService = serviceProvider.GetRequiredService<IQuestionService>();

            QuizEvents = serviceProvider.GetRequiredService<IQuizEvents>();
            RoundEvents = serviceProvider.GetRequiredService<IRoundEvents>();
            QuestionEvents = serviceProvider.GetRequiredService<IQuestionEvents>();
        }

        public async Task<SyncedQuiz> GetAsync(Guid quizId)
        {
            SyncServices services = CreateSyncServices();

            DtoQuiz quiz = await QuizService.GetAsync(quizId);

            List<DtoQuestion> questions = await QuestionService.GetForQuizAsync(quizId);
            var syncedQuestions = questions.Select(q => new SyncedQuestion(q, services)).ToList();

            List<DtoRound> rounds = await RoundService.GetForQuizAsync(quizId);
            var syncedRounds = rounds.Select(r => new SyncedRound(r, syncedQuestions.Where(q => q.RoundId == r.Id), services)).ToList();

            return new SyncedQuiz(quiz, syncedRounds, services);
        }

        private SyncServices CreateSyncServices() =>
            new SyncServices(QuizService, RoundService, QuestionService, QuizEvents, RoundEvents, QuestionEvents);
    }
}
