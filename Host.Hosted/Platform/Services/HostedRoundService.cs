﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Quibble.Core.Entities;
using Quibble.Host.Common;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;
using Quibble.Host.Common.Repositories;
using Quibble.Host.Common.Services;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Services.Data;

namespace Quibble.Host.Hosted.Platform.Services
{
    [Authorize]
    public class HostedRoundService : IRoundService
    {
        private IQuizRepository QuizRepository { get; }
        private IParticipantRepository ParticipantRepository { get; }
        private IRoundRepository RoundRepository { get; }
        private IRoundEventsInvoker RoundEvents { get; }
        private IUserContextAccessor UserContextAccessor { get; }

        public HostedRoundService(IQuizRepository quizRepository,
            IParticipantRepository participantRepository,
            IRoundRepository roundRepository,
            IRoundEventsInvoker roundEvents,
            IUserContextAccessor userContextAccessor)
        {
            QuizRepository = quizRepository;
            ParticipantRepository = participantRepository;
            RoundRepository = roundRepository;
            RoundEvents = roundEvents;
            UserContextAccessor = userContextAccessor;
        }

        public async Task CreateAsync(Guid parentQuizId)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(parentQuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation(ExceptionMessages.CannotEditPublishedQuiz);

            var round = new DbRound {QuizId = quiz.Id};
            await RoundRepository.CreateAsync(round);
            await RoundEvents.InvokeRoundAddedAsync(round);
        }

        public async Task<List<DtoRound>> GetForQuizAsync(Guid quizId)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(quizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
            {
                if (quiz.PublishedAt == null)
                    throw ThrowHelper.NotFound("Quiz", quizId);

                if (!await ParticipantRepository.IsParticipatingAsync(quizId, user.Id))
                    throw ThrowHelper.Unauthorised(ExceptionMessages.NotJoinedQuiz);
            }

            var rounds = await RoundRepository.GetForQuizAsync(quizId);
            return rounds.Select(r => new DtoRound(r)).ToList();
        }

        public async Task DeleteAsync(Guid id)
        {
            DbRound round = await RoundRepository.GetAsync(id);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation(ExceptionMessages.CannotEditPublishedQuiz);

            await RoundRepository.DeleteAsync(id);
            await RoundEvents.InvokeRoundDeletedAsync(id);
        }

        public async Task UpdateTitleAsync(Guid id, string newTitle, Guid initiatorToken)
        {
            newTitle ??= string.Empty;

            DbRound round = await RoundRepository.GetAsync(id);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation(ExceptionMessages.CannotEditPublishedQuiz);

            await RoundRepository.UpdateTitleAsync(id, newTitle);
            await RoundEvents.InvokeTitleUpdatedAsync(id, newTitle, initiatorToken);
        }

        public async Task UpdateStateAsync(Guid id, RoundState newState)
        {
            DbRound round = await RoundRepository.GetAsync(id);
            DbQuiz quiz = await QuizRepository.GetAsync(round.QuizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            if (quiz.PublishedAt == null)
                throw ThrowHelper.InvalidOperation("Cannot update state for an unpublished quiz");

            if (round.State == RoundState.Hidden && newState == RoundState.Visible)
            {
                await RoundRepository.UpdateStateAsync(id, newState);
                await RoundEvents.InvokeStateUpdatedAsync(id, newState);
                return;
            }

            throw ThrowHelper.BadArgument(nameof(newState), "Invalid state progression.");
        }
    }
}
