using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class HostedParticipantService : IParticipantService
    {
        private IQuizRepository QuizRepository { get; }
        private IUserRepository UserRepository { get; }
        private IParticipantRepository ParticipantRepository { get; }
        private IParticipantEventsInvoker ParticipantEvents { get; }
        private IUserContextAccessor UserContextAccessor { get; }

        public HostedParticipantService(IQuizRepository quizRepository, IUserRepository userRepository, IParticipantRepository participantRepository, IParticipantEventsInvoker participantEvents, IUserContextAccessor userContextAccessor)
        {
            QuizRepository = quizRepository;
            UserRepository = userRepository;
            ParticipantRepository = participantRepository;
            ParticipantEvents = participantEvents;
            UserContextAccessor = userContextAccessor;
        }

        public async Task JoinAsync(Guid quizId)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(quizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (user.Id == quiz.OwnerId)
                throw ThrowHelper.InvalidOperation("You own this quiz.");

            var currentParticipants = await ParticipantRepository.GetForQuizAsync(quizId);
            if (currentParticipants.Any(p => p.UserId == user.Id))
                throw ThrowHelper.InvalidOperation("You have already joined this quiz.");

            var participant = new DbParticipant {QuizId = quizId, UserId = user.Id, UserName = user.UserName};
            await ParticipantRepository.CreateAsync(participant);
            await ParticipantEvents.InvokeParticipantJoinedAsync(participant);
        }

        public async Task<List<DtoParticipant>> GetForQuizAsync(Guid quizId)
        {
            DbQuiz quiz = await QuizRepository.GetAsync(quizId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (quiz.PublishedAt == null && user.Id != quiz.OwnerId)
                throw ThrowHelper.NotFound("Quiz", quizId);

            var participants = await ParticipantRepository.GetForQuizAsync(quizId);
            return participants.Select(p => new DtoParticipant(p)).ToList();
        }

        public async Task<string> GetNameAsync(Guid participantId)
        {
            DbParticipant participant = await ParticipantRepository.GetAsync(participantId);
            DbQuibbleUser user = await UserRepository.GetAsync(participant.UserId);
            return user.UserName;
        }

        public async Task KickAsync(Guid participantId)
        {
            DbParticipant participant = await ParticipantRepository.GetAsync(participantId);
            DbQuibbleUser user = await UserContextAccessor.EnsureCurrentUserAsync();

            if (participant.Quiz.OwnerId != user.Id)
                throw ThrowHelper.Unauthorised(ExceptionMessages.NotQuizOwner);

            await ParticipantRepository.DeleteAsync(participantId);
            await ParticipantEvents.InvokeParticipantLeftAsync(participantId);
        }
    }
}
