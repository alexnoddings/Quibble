using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Entities;
using Quibble.Client.Core.Entities.Synced;
using Quibble.Common.Api;

namespace Quibble.Client.Core.Factories;

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

		Logger.LogDebug("Creating synced quiz {QuizId}.", id);

		var syncedQuiz = new SyncedQuiz(EntityLogger, context, dto.Quiz);

		Dictionary<Guid, SyncedParticipant> syncedParticipants = new();
		Dictionary<Guid, int> syncedParticipantOrders = new();
		var participantCount = 0;
		foreach (var participant in dto.Participants.OrderBy(p => p.UserName))
		{
			Logger.LogDebug("Creating participant {ParticipantId}, is current user: {IsCurrentUser}.", participant.Id, participant.IsCurrentUser);
			var syncedParticipant = new SyncedParticipant(EntityLogger, context, participant, syncedQuiz);
			syncedParticipants.Add(syncedParticipant.Id, syncedParticipant);
			syncedQuiz.SyncedParticipants.AddSilent(syncedParticipant);
			participantCount++;
			syncedParticipantOrders.Add(participant.Id, participantCount);
		}

		foreach (var round in dto.Rounds.OrderBy(r => r.Order))
		{
			Logger.LogDebug("Creating round #{RoundOrder} {RoundId}.", round.Order, round.Id);
			var syncedRound = new SyncedRound(EntityLogger, context, round, syncedQuiz);
			foreach (var question in dto.Questions.Where(q => q.RoundId == round.Id).OrderBy(q => q.Order))
			{
				Logger.LogDebug("Creating question #{QuestionOrder} {QuestionId}.", question.Order, question.Id);
				var syncedQuestion = new SyncedQuestion(EntityLogger, context, question, syncedRound);
				foreach (var submittedAnswer in dto.SubmittedAnswers.Where(a => a.QuestionId == question.Id).OrderBy(a => syncedParticipantOrders[a.ParticipantId]))
				{
					Logger.LogDebug("Creating submitted answer {SubmittedAnswerId}.", submittedAnswer.Id);
					var syncedParticipant = syncedParticipants[submittedAnswer.ParticipantId];
					var syncedSubmittedAnswer = new SyncedSubmittedAnswer(EntityLogger, context, submittedAnswer, syncedQuestion, syncedParticipant);
					syncedQuestion.SyncedAnswers.AddSilent(syncedSubmittedAnswer);
					syncedParticipant.SyncedAnswers.AddSilent(syncedSubmittedAnswer);
				}
				syncedRound.SyncedQuestions.AddSilent(syncedQuestion);
			}
			syncedQuiz.SyncedRounds.AddSilent(syncedRound);
		}

		return ApiResponse.FromSuccess<ISyncedQuiz>(syncedQuiz);
	}
}
