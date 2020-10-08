using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quibble.UI.Core.Entities;

namespace Quibble.UI.Core.Services
{
    public interface IParticipantService
    {
        public Task JoinAsync(Guid quizId);

        public Task<List<DtoParticipant>> GetForQuizAsync(Guid quizId);

        public Task KickAsync(Guid participantId);
    }
}
