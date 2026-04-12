using Microsoft.AspNetCore.SignalR;

namespace Quibble.Games.Events;

public interface IGameEventsClients
{
    // Game
    [HubMethodName("gameStateChanged")]
    public Task GameStateChangedAsync(GameStateChangedEvent @event);

    [HubMethodName("gameTitleChanged")]
    public Task GameTitleChangedAsync(GameTitleChangedEvent @event);

    // Participant
    [HubMethodName("participantAdded")]
    public Task ParticipantAddedAsync(ParticipantAddedEvent @event);

    // Round
    [HubMethodName("roundAdded")]
    public Task RoundAddedAsync(RoundAddedEvent @event);

    [HubMethodName("roundRemoved")]
    public Task RoundRemovedAsync(RoundRemovedEvent @event);

    [HubMethodName("roundOrderChanged")]
    public Task RoundOrderChangedAsync(RoundOrderChangedEvent @event);

    [HubMethodName("roundStateChanged")]
    public Task RoundStateChangedAsync(RoundStateChangedEvent @event);

    [HubMethodName("roundTitleChanged")]
    public Task RoundTitleChangedAsync(RoundTitleChangedEvent @event);

    [HubMethodName("roundDescriptionChanged")]
    public Task RoundDescriptionChangedAsync(RoundDescriptionChangedEvent @event);

    // Question
    [HubMethodName("questionAdded")]
    public Task QuestionAddedAsync(QuestionAddedEvent @event);

    [HubMethodName("questionRevealed")]
    public Task QuestionRevealedAsync(QuestionRevealedEvent @event);

    [HubMethodName("questionRemoved")]
    public Task QuestionRemovedAsync(QuestionRemovedEvent @event);

    [HubMethodName("questionOrderChanged")]
    public Task QuestionOrderChangedAsync(QuestionOrderChangedEvent @event);

    [HubMethodName("questionStateChanged")]
    public Task QuestionStateChangedAsync(QuestionStateChangedEvent @event);

    [HubMethodName("questionPointsChanged")]
    public Task QuestionPointsChangedAsync(QuestionPointsChangedEvent @event);

    [HubMethodName("questionBodyTextChanged")]
    public Task QuestionBodyTextChangedAsync(QuestionBodyTextChangedEvent @event);

    [HubMethodName("questionAnswerTextChanged")]
    public Task QuestionAnswerTextChangedAsync(QuestionAnswerTextChangedEvent @event);

    // Answer
    [HubMethodName("answerTextChanged")]
    public Task AnswerTextChangedAsync(AnswerTextChangedEvent @event);

    [HubMethodName("answerTextPreviewed")]
    public Task AnswerTextPreviewedAsync(AnswerTextPreviewedEvent @event);

    [HubMethodName("answerPointsChanged")]
    public Task AnswerPointsChangedAsync(AnswerPointsChangedEvent @event);
}
