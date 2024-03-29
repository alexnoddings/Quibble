﻿namespace Quibble.Common.Entities;

public interface IQuestion : IEntity
{
	public Guid RoundId { get; }
	public string Text { get; }
	public string Answer { get; }
	public decimal Points { get; }
	public QuestionState State { get; }
	public int Order { get; }
}
