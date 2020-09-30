﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Quibble.Host.Common;
using Quibble.Host.Common.Data.Entities;
using Quibble.Host.Common.Extensions;
using Quibble.Host.Common.Repositories;
using Quibble.Host.Common.Services;
using Quibble.Host.Hosted.Platform.Events;
using Quibble.UI.Core.Entities;
using Quibble.UI.Core.Services;

namespace Quibble.Host.Hosted.Platform.Services
{
    [Authorize]
    public class HostedQuizService : IQuizService
    {
        private IQuizRepository QuizRepository { get; }
        private IQuizEventsInvoker QuizEvents { get; }
        private IUserContextAccessor UserContextAccessor { get; }

        public HostedQuizService(IQuizRepository quizRepository, IQuizEventsInvoker quizEvents, IUserContextAccessor userContextAccessor)
        {
            QuizRepository = quizRepository;
            QuizEvents = quizEvents;
            UserContextAccessor = userContextAccessor;
        }

        public async Task<Guid> CreateAsync(string title)
        {
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();
            var quiz = new DbQuiz
            {
                OwnerId = user.Id,
                Title = title ?? string.Empty
            };
            await QuizRepository.CreateAsync(quiz);
            return quiz.Id;
        }

        public async Task<DtoQuiz> GetAsync(Guid id)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(id);

            if (quiz.PublishedAt == null)
            {
                DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();
                if (user.Id != quiz.OwnerId)
                    throw ThrowHelper.NotFound("Quiz", id);
            }

            return new DtoQuiz(quiz);
        }

        public async Task PublishAsync(Guid id)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(id);

            if (quiz.PublishedAt != null)
                throw ThrowHelper.InvalidOperation("The quiz is already published.");

            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();
            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            DateTime publishedAt = await QuizRepository.PublishAsync(id);
            await QuizEvents.InvokePublishedAsync(id, publishedAt);
        }

        public async Task DeleteAsync(Guid id)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(id);

            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();
            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            await QuizRepository.DeleteAsync(id);
            await QuizEvents.InvokeDeletedAsync(id);
        }

        public async Task UpdateTitleAsync(Guid id, string newTitle)
        {
            newTitle ??= string.Empty;

            DbQuiz quiz = await QuizRepository.GetAsync(id);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (user.Id != quiz.OwnerId)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            await QuizRepository.UpdateTitleAsync(id, newTitle);
            await QuizEvents.InvokeTitleUpdatedAsync(id, newTitle);
        }
    }
}