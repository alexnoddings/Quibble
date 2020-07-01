using System;
using System.Collections.Generic;
using Quibble.Server.Models.Questions;
using Quibble.Server.Models.Quizzes;

namespace Quibble.Server.Models.Rounds
{
    public class Round : IEntity<Guid>
    {
        public Guid Id { get; }
        public Guid QuizId { get; }

        public string Title { get; set; } = "New Round";
        public RoundState State { get; set; } = RoundState.Hidden;
    }
}
