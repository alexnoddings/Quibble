using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Core;
using Quibble.Client.Sync.Entities;
using Quibble.Shared.Api;

namespace Quibble.Client.Sync
{
    internal class SyncedQuizFactory : ISyncedQuizFactory
    {
        private ILogger<SyncedQuizFactory> Logger { get; }
        private ILogger<SyncedEntity> EntityLogger { get; }
        private ISyncedQuizService Service { get; }

        public SyncedQuizFactory(ILogger<SyncedQuizFactory> logger, ILogger<SyncedEntity> entityLogger, ISyncedQuizService service)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            EntityLogger = entityLogger ?? throw new ArgumentNullException(nameof(entityLogger));
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<ApiResponse<ISyncedQuiz>> GetSyncedQuizAsync(Guid id)
        {
            Logger.LogInformation("Creating synchronised quiz {QuizId}.", id);

            var serviceResponse = await Service.GetQuizAsync(id);
            if (!serviceResponse.WasSuccessful)
            {
                Logger.LogWarning("Failed to get quiz from {SyncServiceName}: {Error}", nameof(ISyncedQuizService), serviceResponse.Error);
                return ApiResponse.FromError<ISyncedQuiz>(serviceResponse.Error);
            }

            var (dto, context) = serviceResponse.Value;

            Logger.LogDebug("Creating quiz.");

            var syncedQuiz = new SyncedQuiz(EntityLogger, context, dto.Quiz);

            Dictionary<Guid, SyncedParticipant> syncedParticipants = new();
            foreach (var participant in dto.Participants)
            {
                Logger.LogDebug("Creating participant {ParticipantId}, is current user: {IsCurrentUser}.", participant.Id, participant.IsCurrentUser);
                var syncedParticipant = new SyncedParticipant(EntityLogger, context, participant, syncedQuiz);
                syncedParticipants.Add(syncedParticipant.Id, syncedParticipant);
                await syncedQuiz.SyncedParticipants.AddAsync(syncedParticipant);
            }

            foreach (var round in dto.Rounds)
            {
                Logger.LogDebug("Creating round #{RoundOrder} {RoundId}.", round.Order, round.Id);
                var syncedRound = new SyncedRound(EntityLogger, context, round, syncedQuiz);
                foreach (var question in dto.Questions.Where(q => q.RoundId == round.Id))
                {
                    Logger.LogDebug("Creating question #{QuestionOrder} {QuestionId}.", question.Order, question.Id);
                    var syncedQuestion = new SyncedQuestion(EntityLogger, context, question, syncedRound);
                    foreach (var submittedAnswer in dto.SubmittedAnswers.Where(a => a.QuestionId == question.Id))
                    {
                        Logger.LogDebug("Creating submitted answer {SubmittedAnswerId}.", submittedAnswer.Id);
                        var syncedParticipant = syncedParticipants[submittedAnswer.ParticipantId];
                        var syncedSubmittedAnswer = new SyncedSubmittedAnswer(EntityLogger, context, submittedAnswer, syncedQuestion, syncedParticipant);
                        await syncedQuestion.SyncedAnswers.AddAsync(syncedSubmittedAnswer);
                        await syncedParticipant.SyncedAnswers.AddAsync(syncedSubmittedAnswer);
                    }
                    await syncedRound.SyncedQuestions.AddAsync(syncedQuestion);
                }
                await syncedQuiz.SyncedRounds.AddAsync(syncedRound);
            }

            return ApiResponse.FromSuccess<ISyncedQuiz>(syncedQuiz);
        }
    }
}
