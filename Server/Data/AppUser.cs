using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Quibble.Server.Data.Models;
using Quibble.Shared.Entities;

namespace Quibble.Server.Data
{
    public class AppUser : IdentityUser<Guid>, IEntity
    {
        public static readonly Guid DeletedUserId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

        public List<DbQuiz> Quizzes { get; set; } = new();
        public List<DbParticipant> Participations { get; set; } = new();
        public List<DbSubmittedAnswer> SubmittedAnswers { get; set; } = new();
    }
}
