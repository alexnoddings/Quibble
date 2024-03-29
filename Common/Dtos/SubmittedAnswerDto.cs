﻿using Quibble.Common.Entities;

namespace Quibble.Common.Dtos;

public class SubmittedAnswerDto : ISubmittedAnswer
{
	public Guid Id { get; set; }
	public Guid QuestionId { get; set; }
	public Guid ParticipantId { get; set; }
	public string Text { get; set; } = string.Empty;
	public decimal AssignedPoints { get; set; }
}
