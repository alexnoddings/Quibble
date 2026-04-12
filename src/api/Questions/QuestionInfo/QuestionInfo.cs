using System.ComponentModel;
using Quibble.Data.Entites.Questions;
using Quibble.Rounds.Info;

namespace Quibble.Questions.Info;

[ImmutableObject(true)]
public sealed class QuestionInfo
{
    public required RoundInfo Round { get; init; }

    public required Guid Id { get; init; }
    public required QuestionState State { get; init; }
}
