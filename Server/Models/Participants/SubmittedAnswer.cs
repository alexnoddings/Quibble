﻿using System;
using Quibble.Server.Models.Questions;

namespace Quibble.Server.Models.Users
{
    public class SubmittedAnswer : IEntity<Guid>
    {
        public Guid Id { get; }
        public Guid ParticipantId { get; set; }
        public Guid QuestionId { get; set; }

        public SubmittedAnswerState State { get; set; } = SubmittedAnswerState.Unmarked;
        public string Value { get; set; } = string.Empty;
    }
}
