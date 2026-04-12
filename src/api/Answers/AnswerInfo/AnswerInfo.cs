using System.ComponentModel;
using Quibble.Games.Info;

namespace Quibble.Questions.Info;

[ImmutableObject(true)]
public sealed class AnswerInfo
{
    public required GameParticipantInfo Participant { get; init; }
    public required QuestionInfo Question { get; init; }
}
