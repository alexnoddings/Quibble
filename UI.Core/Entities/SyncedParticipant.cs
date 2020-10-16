using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;

namespace Quibble.UI.Core.Entities
{
    public class SyncedParticipant : IParticipant
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid QuizId { get; }
        public string UserName { get; }

        private readonly AsyncEvent _updated = new();
        public event Func<Task> Updated
        {
            add => _updated.Add(value);
            remove => _updated.Remove(value);
        }

        private readonly SyncServices _services;

        internal SyncedParticipant(IParticipant dtoParticipant, SyncServices services)
        {
            _services = services;

            Id = dtoParticipant.Id;
            UserId = dtoParticipant.UserId;
            QuizId = dtoParticipant.QuizId;
            UserName = dtoParticipant.UserName;
        }

        public Task KickAsync() => _services.ParticipantService.KickAsync(Id);
    }
}
