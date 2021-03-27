using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Quibble.Server.Data.Models;
using Quibble.Shared.Models;

namespace Quibble.Server.Data
{
    public class AppUser : IdentityUser<Guid>, IEntity
    {
        public List<DbQuiz> Quizzes { get; set; } = default!;
        public List<DbQuiz> ParticipatedIn { get; set; } = default!;
        public List<DbSubmittedAnswer> SubmittedAnswers { get; set; } = default!;
    }
}
