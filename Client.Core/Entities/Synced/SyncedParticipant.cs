using Microsoft.Extensions.Logging;
using Quibble.Client.Core.Contexts;
using Quibble.Common.Dtos;

namespace Quibble.Client.Core.Entities.Synced;

internal sealed class SyncedParticipant : SyncedEntity, ISyncedParticipant
{
	public string UserName { get; }
	public bool IsCurrentUser { get; }

	internal SyncedQuiz SyncedQuiz { get; }
	public ISyncedQuiz Quiz => SyncedQuiz;
	public Guid QuizId => SyncedQuiz.Id;

	internal SyncedEntitiesList<SyncedSubmittedAnswer> SyncedAnswers { get; } = new();
	public ISyncedEntities<ISyncedSubmittedAnswer> Answers => SyncedAnswers;

	public SyncedParticipant(ILogger<SyncedEntity> logger, ISyncContext syncContext, ParticipantDto participant, SyncedQuiz syncedQuiz)
		: base(logger, syncContext)
	{
		Id = participant.Id;
		UserName = participant.UserName;
		IsCurrentUser = participant.IsCurrentUser;

		SyncedQuiz = syncedQuiz;
	}

	public override int GetStateStamp() =>
		StateStamp.ForProperties(SyncedAnswers);
}
