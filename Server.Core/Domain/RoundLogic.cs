using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quibble.Common.Api;
using Quibble.Common.Dtos;
using Quibble.Common.Entities;
using Quibble.Server.Core;
using Quibble.Server.Core.Models;
using Quibble.Server.Core.Sync;

namespace Quibble.Server.Core.Domain;

public partial class RoundLogic : BaseLogic
{
	public RoundLogic(ILogger<RoundLogic> logger, IMapper mapper, AppDbContext dbContext)
		: base(logger, mapper, dbContext)
	{
	}

	public async Task<ApiResponse> CreateRoundAsync(IExecutionContext context)
	{
		var dbQuiz =
			await DbContext.Quizzes
				.Include(quiz => quiz.Rounds)
				.FindAsync(context.QuizId);

		if (dbQuiz is null)
			return Failure(ApiErrors.RoundParentQuizNotFound);

		if (dbQuiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbQuiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		var dbRound = new DbRound
		{
			Title = string.Empty,
			State = RoundState.Hidden,
			Order = dbQuiz.Rounds.Count
		};
		dbQuiz.Rounds.Add(dbRound);
		await DbContext.SaveChangesAsync();

		var round = Mapper.Map<RoundDto>(dbRound);
		await context.Users.All().OnRoundAddedAsync(round);

		return Success();
	}

	public async Task<ApiResponse> UpdateRoundTitleAsync(IExecutionContext context, Guid roundId, string newTitle)
	{
		var dbRound =
			await DbContext.Rounds
				.Include(round => round.Quiz)
				.FindAsync(roundId);

		if (dbRound is null)
			return Failure(ApiErrors.RoundNotFound);

		if (dbRound.QuizId != context.QuizId)
			return Failure(ApiErrors.RoundNotFound);

		if (dbRound.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbRound.Quiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		newTitle ??= string.Empty;
		if (newTitle.Length > 100)
			return Failure(ApiErrors.RoundTitleTooLong);

		dbRound.Title = newTitle;
		await DbContext.SaveChangesAsync();

		await context.Users.All().OnRoundTitleUpdatedAsync(dbRound.Id, newTitle);

		return Success();
	}

	public async Task<ApiResponse> OpenRoundAsync(IExecutionContext context, Guid roundId)
	{
		var dbRound =
			await DbContext.Rounds
				.Include(round => round.Quiz)
				.FindAsync(roundId);

		if (dbRound is null)
			return Failure(ApiErrors.RoundNotFound);

		if (dbRound.QuizId != context.QuizId)
			return Failure(ApiErrors.RoundNotFound);

		if (dbRound.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbRound.Quiz.State != QuizState.Open)
			return Failure(ApiErrors.CantUpdateAsNotOpen);

		dbRound.State = RoundState.Open;
		await DbContext.SaveChangesAsync();

		await context.Users.Hosts().OnRoundOpenedAsync(dbRound.Id);

		var round = Mapper.Map<RoundDto>(dbRound);
		await context.Users.Participants().OnRoundAddedAsync(round);

		return Success();
	}

	public async Task<ApiResponse> DeleteRoundAsync(IExecutionContext context, Guid roundId)
	{
		var dbRound =
			await DbContext.Rounds
				.Include(round => round.Quiz)
					.ThenInclude(quiz => quiz.Rounds)
				.FindAsync(roundId);

		if (dbRound is null)
			return Failure(ApiErrors.RoundNotFound);

		if (dbRound.QuizId != context.QuizId)
			return Failure(ApiErrors.RoundNotFound);

		if (dbRound.Quiz.OwnerId != context.UserId)
			return Failure(ApiErrors.CantEditAsNotOwner);

		if (dbRound.Quiz.State != QuizState.InDevelopment)
			return Failure(ApiErrors.CantDeleteAsNotInDevelopment);

		DbContext.Rounds.Remove(dbRound);
		dbRound.Quiz.Rounds.Remove(dbRound);

		List<DbRound> modifiedRounds = new(dbRound.Quiz.Rounds.Count);
		var roundCount = 0;
		foreach (var quizRound in dbRound.Quiz.Rounds.OrderBy(round => round.Order))
		{
			if (quizRound.Order != roundCount)
			{
				quizRound.Order = roundCount;
				modifiedRounds.Add(quizRound);
			}
			roundCount++;
		}
		await DbContext.SaveChangesAsync();

		await context.Users.All().OnRoundDeletedAsync(dbRound.Id);
		foreach (var modifiedRound in modifiedRounds)
			await context.Users.All().OnRoundOrderUpdatedAsync(modifiedRound.Id, modifiedRound.Order);

		return Success();
	}
}
