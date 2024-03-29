﻿using Microsoft.Extensions.Logging;
using Quibble.Client.Core;
using Quibble.Client.Core.Contexts;
using Quibble.Client.Core.Entities;
using Quibble.Common.Dtos;
using Quibble.Common.Entities;

namespace Quibble.Client.Core.Entities.Synced;

internal sealed class SyncedRound : SyncedEntity, ISyncedRound, IDisposable
{
	public string Title { get; private set; }
	public RoundState State { get; private set; }
	public int Order { get; private set; }

	internal SyncedQuiz SyncedQuiz { get; }
	public ISyncedQuiz Quiz => SyncedQuiz;
	public Guid QuizId => SyncedQuiz.Id;

	internal SyncedEntitiesList<SyncedQuestion> SyncedQuestions { get; } = new();
	public ISyncedEntities<ISyncedQuestion> Questions => SyncedQuestions;

	internal SyncedRound(ILogger<SyncedEntity> logger, ISyncContext syncContext, RoundDto round, SyncedQuiz syncedQuiz)
		: base(logger, syncContext)
	{
		Id = round.Id;
		Title = round.Title;
		State = round.State;
		Order = round.Order;

		SyncedQuiz = syncedQuiz;

		SyncContext.Rounds.OnOpenedAsync += OnOpenedAsync;
		SyncContext.Rounds.OnTitleUpdatedAsync += OnTitleUpdatedAsync;
		SyncContext.Rounds.OnOrderUpdatedAsync += OnOrderUpdatedAsync;

		SyncContext.Questions.OnAddedAsync += OnQuestionAddedAsync;
		SyncContext.Questions.OnRevealedAsync += OnQuestionRevealedAsync;
		SyncContext.Questions.OnDeletedAsync += OnQuestionDeletedAsync;
	}

	public Task AddQuestionAsync() =>
		SyncContext.Questions.AddQuestionAsync(Id);

	public Task UpdateTitleAsync(string newTitle) =>
		SyncContext.Rounds.UpdateTitleAsync(Id, newTitle);

	public Task OpenAsync() =>
		SyncContext.Rounds.OpenAsync(Id);

	public Task DeleteAsync() =>
		SyncContext.Rounds.DeleteAsync(Id);

	private Task OnOpenedAsync(Guid id)
	{
		if (id != Id)
			return Task.CompletedTask;

		State = RoundState.Open;
		return OnUpdatedAsync();
	}

	private Task OnTitleUpdatedAsync(Guid id, string newTitle)
	{
		if (id != Id)
			return Task.CompletedTask;

		Title = newTitle;
		return OnUpdatedAsync();
	}

	private Task OnOrderUpdatedAsync(Guid id, int newOrder)
	{
		if (id != Id)
			return Task.CompletedTask;

		Order = newOrder;
		return OnUpdatedAsync();
	}

	private async Task OnQuestionAddedAsync(QuestionDto questionDto)
	{
		if (questionDto.RoundId != Id)
			return;

		await SyncedQuestions.AddAsync(new SyncedQuestion(Logger, SyncContext, questionDto, this));
		await OnUpdatedAsync();
	}

	private async Task OnQuestionRevealedAsync(QuestionDto questionDto, List<SubmittedAnswerDto> answers)
	{
		if (questionDto.RoundId != Id)
			return;

		var question = new SyncedQuestion(Logger, SyncContext, questionDto, this);
		foreach (var answer in answers)
		{
			var participant = SyncedQuiz.SyncedParticipants.FirstOrDefault(p => p.Id == answer.ParticipantId);
			if (participant is null) continue;
			var syncedAnswer = new SyncedSubmittedAnswer(Logger, SyncContext, answer, question, participant);
			await question.SyncedAnswers.AddAsync(syncedAnswer);
		}
		await SyncedQuestions.AddAsync(question);
		await OnUpdatedAsync();
	}

	private async Task OnQuestionDeletedAsync(Guid questionId)
	{
		var question = SyncedQuestions.FirstOrDefault(question => question.Id == questionId);
		if (question is null)
			return;

		await SyncedQuestions.RemoveAsync(question);
		await OnUpdatedAsync();
	}

	public override int GetStateStamp() =>
		StateStamp.ForProperties(Title, State, Order, Questions);

	public void Dispose()
	{
		SyncContext.Rounds.OnOpenedAsync -= OnOpenedAsync;
		SyncContext.Rounds.OnTitleUpdatedAsync -= OnTitleUpdatedAsync;
		SyncContext.Rounds.OnOrderUpdatedAsync -= OnOrderUpdatedAsync;

		SyncContext.Questions.OnAddedAsync -= OnQuestionAddedAsync;
		SyncContext.Questions.OnDeletedAsync -= OnQuestionDeletedAsync;
	}
}
