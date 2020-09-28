using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Quibble.Core.Entities;

namespace Quibble.Host.Common.Data.Entities
{
    public class DbQuibbleUser : IdentityUser<Guid>, IQuibbleUser
    {
        public DateTime Joined { get; set; } = DateTime.UtcNow;

        public List<DbQuiz> Quizzes { get; set; } = new List<DbQuiz>();

        public List<DbParticipant> Participants { get; set; } = new List<DbParticipant>();
    }
}