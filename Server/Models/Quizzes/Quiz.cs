using System;
using System.Collections.Generic;
using Quibble.Server.Models.Rounds;
using Quibble.Server.Models.Users;

namespace Quibble.Server.Models.Quizzes
{
    public class Quiz : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;

        public string Title { get; set; } = "New Quiz";
        public QuizState State { get; set; } = QuizState.InDevelopment;
    }
}
