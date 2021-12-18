using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quibble.Common.Api;
using Quibble.Common.Entities;
using Quibble.Server.Core;
using Quibble.Server.Core.Sync;

namespace Quibble.Server.Core.Domain;

public partial class SubmittedAnswerLogic : BaseLogic
{
	public SubmittedAnswerLogic(ILogger<SubmittedAnswerLogic> logger, IMapper mapper, AppDbContext dbContext)
		: base(logger, mapper, dbContext)
	{
	}

	private async Task<ApiResponse> UpdateSubmittedAnswerTextCoreAsync(IExecutionContext context, Guid answerId, string newText, bool shouldPersist)
	{
		var dbAnswer =
			await DbContext.SubmittedAnswers
				.Include(answer => answer.Participant)
				.Include(answer => answer.Question)
					.ThenInclude(question => question.Round)
						.ThenInclude(round => round.Quiz)
				.FindAsync(answerId);

		if (dbAnswer is null)
			return Failure(ApiErrors.AnswerNotFound);

		if (dbAnswer.Question.Round.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionParentRoundNotFound);

		if (dbAnswer.Participant.UserId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbAnswer.Question.State != QuestionState.Open)
			return Failure(ApiErrors.QuestionBadState);

		newText ??= string.Empty;
		if (newText.Length > 200)
			return Failure(ApiErrors.TextTooLong);

		if (shouldPersist)
		{
			dbAnswer.Text = newText;
			await DbContext.SaveChangesAsync();
		}
		await context.Users.Hosts().OnSubmittedAnswerTextUpdatedAsync(answerId, newText);
		var quizParticipantExceptConnection = context.Users.ParticipantExceptCaller(dbAnswer.ParticipantId);
		await quizParticipantExceptConnection.OnSubmittedAnswerTextUpdatedAsync(answerId, newText);

		return Success();
	}

	public Task<ApiResponse> PreviewUpdateSubmittedAnswerTextAsync(IExecutionContext context, Guid answerId, string newText) =>
		UpdateSubmittedAnswerTextCoreAsync(context, answerId, newText, false);

	public Task<ApiResponse> UpdateSubmittedAnswerTextAsync(IExecutionContext context, Guid answerId, string newText) =>
		UpdateSubmittedAnswerTextCoreAsync(context, answerId, newText, true);

	public async Task<ApiResponse> UpdateSubmittedAnswerAssignedPointsAsync(IExecutionContext context, Guid answerId, decimal points)
	{
		var dbAnswer =
			await DbContext.SubmittedAnswers
				.Include(answer => answer.Participant)
				.Include(answer => answer.Question)
					.ThenInclude(question => question.Round)
						.ThenInclude(round => round.Quiz)
				.FindAsync(answerId);

		if (dbAnswer is null)
			return Failure(ApiErrors.AnswerNotFound);

		if (dbAnswer.Question.Round.QuizId != context.QuizId)
			return Failure(ApiErrors.QuestionParentRoundNotFound);

		if (dbAnswer.Question.Round.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbAnswer.Question.State < QuestionState.Locked)
			return Failure(ApiErrors.QuestionBadState);

		if (points < 0)
			return Failure(ApiErrors.PointsTooLow);

		if (points > 10m)
			return Failure(ApiErrors.PointsTooHigh);

		// Ensure points are a division of 0.25
		points = Math.Round(points * 4, MidpointRounding.ToEven) / 4;
		dbAnswer.AssignedPoints = points;
		await DbContext.SaveChangesAsync();

		await context.Users.Hosts().OnSubmittedAnswerAssignedPointsUpdatedAsync(answerId, points);
		if (dbAnswer.Question.State == QuestionState.AnswerRevealed)
			await context.Users.Participant(dbAnswer.ParticipantId).OnSubmittedAnswerAssignedPointsUpdatedAsync(answerId, points);

		return Success();
	}
}
