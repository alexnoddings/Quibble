using Quibble.Data.Entites.Answers;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Rounds;
using Quibble.Questions.Info;
using Quibble.Rounds.Info;

namespace Quibble.Games.Events;

public readonly ref struct GameEventsGroup
{
    private readonly IGameEventsClients _clients;

    public GameEventsGroup(IGameEventsClients clients)
    {
        _clients = clients;
    }

    public ScopedGameEvents Game() => new(_clients);
    public ScopedParticipantEvents Participant(Guid participantId) => new(_clients, participantId);

    public ScopedRoundEvents Round(Guid roundId) => new(_clients, roundId);
    public ScopedRoundEvents Round(RoundInfo round) => new(_clients, round.Id);

    public ScopedQuestionEvents Question(Guid questionId) => new(_clients, questionId);
    public ScopedQuestionEvents Question(QuestionInfo question) => new(_clients, question.Id);

    public ScopedAnswerEvents Answer(Guid participantId, Guid questionId) => new(_clients, participantId, questionId);
    public ScopedAnswerEvents Answer(AnswerInfo answer) => new(_clients, answer.Participant.Id, answer.Question.Id);
}

public readonly ref struct ScopedGameEvents
{
    private readonly IGameEventsClients _clients;

    public ScopedGameEvents(IGameEventsClients clients)
    {
        _clients = clients;
    }

    public Task StateChangedAsync(GameState newState) =>
        _clients.GameStateChangedAsync(new() { State = newState });

    public Task TitleChangedAsync(string newTitle) =>
        _clients.GameTitleChangedAsync(new() { Title = newTitle });
}

public readonly ref struct ScopedParticipantEvents
{
    private readonly IGameEventsClients _clients;
    private readonly Guid _participantId;

    public ScopedParticipantEvents(IGameEventsClients clients, Guid participantId)
    {
        _clients = clients;
        _participantId = participantId;
    }

    public Task AddedAsync(string name) =>
        _clients.ParticipantAddedAsync(new()
        {
            ParticipantId = _participantId,
            Name = name,
            Answers = []
        });

    public Task AddedAsync(string name, List<SubmittedAnswer>? answers) =>
        _clients.ParticipantAddedAsync(new()
        {
            ParticipantId = _participantId,
            Name = name,
            Answers = answers?.Select(a => new ParticipantAddedAnswer
            {
                QuestionId = a.QuestionAnswerId,
                Answer = a.Answer
            }).ToList()
        });
}

public readonly ref struct ScopedRoundEvents
{
    private readonly IGameEventsClients _clients;
    private readonly Guid _roundId;

    public ScopedRoundEvents(IGameEventsClients clients, Guid roundId)
    {
        _clients = clients;
        _roundId = roundId;
    }

    public Task AddedAsync(int order, RoundState state, string title, string description) =>
        _clients.RoundAddedAsync(new()
        {
            RoundId = _roundId,
            Order = order,
            State = state,
            Title = title,
            Description = description
        });

    public Task RemovedAsync() =>
        _clients.RoundRemovedAsync(new() { RoundId = _roundId });

    public Task OrderChangedAsync(int order) =>
        _clients.RoundOrderChangedAsync(new()
        {
            RoundId = _roundId,
            Order = order
        });

    public Task StateChangedAsync(RoundState state) =>
        _clients.RoundStateChangedAsync(new()
        {
            RoundId = _roundId,
            State = state
        });

    public Task TitleChangedAsync(string title) =>
        _clients.RoundTitleChangedAsync(new()
        {
            RoundId = _roundId,
            Title = title
        });

    public Task DescriptionChangedAsync(string description) =>
        _clients.RoundDescriptionChangedAsync(new()
        {
            RoundId = _roundId,
            Description = description
        });

}

public readonly ref struct ScopedQuestionEvents
{
    private readonly IGameEventsClients _clients;
    private readonly Guid _questionId;

    public ScopedQuestionEvents(IGameEventsClients clients, Guid questionId)
    {
        _clients = clients;
        _questionId = questionId;
    }

    public Task AddedAsync(Guid roundId, int order, QuestionState state, decimal points, string body, string answer) =>
        _clients.QuestionAddedAsync(new()
        {
            QuestionId = _questionId,
            RoundId = roundId,
            Order = order,
            State = state,
            Points = points,
            BodyText = body,
            AnswerText = answer
        });

    public Task RevealedAsync(string body, string answer) =>
        _clients.QuestionRevealedAsync(new()
        {
            QuestionId = _questionId,
            BodyText = body,
            AnswerText = answer
        });

    public Task RemovedAsync() =>
        _clients.QuestionRemovedAsync(new() { QuestionId = _questionId });

    public Task OrderChangedAsync(int order) =>
        _clients.QuestionOrderChangedAsync(new()
        {
            QuestionId = _questionId,
            Order = order
        });

    public Task StateChangedAsync(QuestionState state) =>
        _clients.QuestionStateChangedAsync(new()
        {
            QuestionId = _questionId,
            State = state
        });

    public Task PointsChangedAsync(decimal points) =>
        _clients.QuestionPointsChangedAsync(new()
        {
            QuestionId = _questionId,
            Points = points
        });

    public Task BodyTextChangedAsync(string body) =>
        _clients.QuestionBodyTextChangedAsync(new()
        {
            QuestionId = _questionId,
            BodyText = body
        });

    public Task AnswerTextChangedAsync(string answer) =>
        _clients.QuestionAnswerTextChangedAsync(new()
        {
            QuestionId = _questionId,
            AnswerText = answer
        });
}

public readonly ref struct ScopedAnswerEvents
{
    private readonly IGameEventsClients _clients;
    private readonly Guid _participantId;
    private readonly Guid _questionId;

    public ScopedAnswerEvents(IGameEventsClients clients, Guid participantId, Guid questionId)
    {
        _clients = clients;
        _participantId = participantId;
        _questionId = questionId;
    }

    public Task TextChangedAsync(string answer) =>
        _clients.AnswerTextChangedAsync(new()
        {
            ParticipantId = _participantId,
            QuestionId = _questionId,
            Answer = answer
        });

    public Task TextPreviewedAsync(string answer) =>
        _clients.AnswerTextPreviewedAsync(new()
        {
            ParticipantId = _participantId,
            QuestionId = _questionId,
            Answer = answer
        });

    public Task PointsChangedAsync(decimal points) =>
        _clients.AnswerPointsChangedAsync(new()
        {
            ParticipantId = _participantId,
            QuestionId = _questionId,
            Points = points
        });
}
