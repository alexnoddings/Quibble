using Microsoft.Extensions.Logging;
using Quibble.Common.Dtos;
using Quibble.Common.Entities;
using Quibble.Server.Core.Sync;
using Quibble.Sync.InMemory.Client.Contexts;
using System.Runtime.CompilerServices;

namespace Quibble.Sync.InMemory.Server;

internal class SyncedEvents : ISyncedEvents
{
	private ILogger<SyncedEvents> Logger { get; }
	private SyncContextStore ContextStore { get; }
	private Func<SyncContext, bool> TargetPredicate { get; }

	public SyncedEvents(ILoggerFactory loggerFactory, SyncContextStore contextStore, Func<SyncContext, bool> targetPredicate)
	{
		Logger = loggerFactory.CreateLogger<SyncedEvents>();
		ContextStore = contextStore ?? throw new ArgumentNullException(nameof(contextStore));
		TargetPredicate = targetPredicate ?? throw new ArgumentNullException(nameof(targetPredicate));
	}

	public Task OnQuizOpenedAsync() =>
		InvokeAsync(context => context.QuizContext.InvokeOnOpenedAsync());
	public Task OnQuizTitleUpdatedAsync(string newTitle) =>
		InvokeAsync(context => context.QuizContext.InvokeOnTitleUpdatedAsync(newTitle));
	public Task OnQuizDeletedAsync() =>
		InvokeAsync(context => context.QuizContext.InvokeOnDeletedAsync());

	public Task OnParticipantJoinedAsync(ParticipantDto participant, List<SubmittedAnswerDto> submittedAnswers) =>
		InvokeAsync(context => context.ParticipantContext.InvokeOnParticipantJoinedAsync(participant, submittedAnswers));

	public Task OnRoundAddedAsync(RoundDto round) =>
		InvokeAsync(context => context.RoundContext.InvokeOnAddedAsync(round));
	public Task OnRoundOpenedAsync(Guid roundId) =>
		InvokeAsync(context => context.RoundContext.InvokeOnOpenedAsync(roundId));
	public Task OnRoundTitleUpdatedAsync(Guid roundId, string newTitle) =>
		InvokeAsync(context => context.RoundContext.InvokeOnTitleUpdatedAsync(roundId, newTitle));
	public Task OnRoundOrderUpdatedAsync(Guid roundId, int newOrder) =>
		InvokeAsync(context => context.RoundContext.InvokeOnOrderUpdatedAsync(roundId, newOrder));
	public Task OnRoundDeletedAsync(Guid roundId) =>
		InvokeAsync(context => context.RoundContext.InvokeOnDeletedAsync(roundId));

	public Task OnQuestionAddedAsync(QuestionDto question) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnAddedAsync(question));
	public Task OnQuestionTextUpdatedAsync(Guid questionId, string newText) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnTextUpdatedAsync(questionId, newText));
	public Task OnQuestionAnswerUpdatedAsync(Guid questionId, string newAnswer) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnAnswerUpdatedAsync(questionId, newAnswer));
	public Task OnQuestionPointsUpdatedAsync(Guid questionId, decimal newPoints) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnPointsUpdatedAsync(questionId, newPoints));
	public Task OnQuestionStateUpdatedAsync(Guid questionId, QuestionState newState) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnStateUpdatedAsync(questionId, newState));
	public Task OnQuestionRevealedAsync(QuestionDto question, List<SubmittedAnswerDto> submittedAnswers) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnRevealedAsync(question, submittedAnswers));
	public Task OnQuestionOrderUpdatedAsync(Guid questionId, int newOrder) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnOrderUpdatedAsync(questionId, newOrder));
	public Task OnQuestionDeletedAsync(Guid questionId) =>
		InvokeAsync(context => context.QuestionContext.InvokeOnDeletedAsync(questionId));

	public Task OnSubmittedAnswerAssignedPointsUpdatedAsync(Guid submittedAnswerId, decimal newPoints) =>
		InvokeAsync(context => context.SubmittedAnswerContext.InvokeOnAssignedPointsUpdatedAsync(submittedAnswerId, newPoints));
	public Task OnSubmittedAnswerTextUpdatedAsync(Guid submittedAnswerId, string newText) =>
		InvokeAsync(context => context.SubmittedAnswerContext.InvokeOnTextUpdatedAsync(submittedAnswerId, newText));

	private async Task InvokeAsync(Func<SyncContext, Task> func, [CallerMemberName] string caller = "")
	{
		var targets = ContextStore.Where(TargetPredicate).ToList();
		Logger.LogDebug("Dispatching {EventName} across {TargetCount} contexts.", caller, targets.Count);
		foreach (var context in targets)
			await func(context);
	}
}
