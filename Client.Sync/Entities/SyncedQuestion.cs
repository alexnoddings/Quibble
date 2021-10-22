using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Core.Contexts;
using Quibble.Client.Sync.Core.Entities;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Entities;

internal sealed class SyncedQuestion : SyncedEntity, ISyncedQuestion, IDisposable
{
    public string Text { get; private set; }
    public string Answer { get; private set; }
    public decimal Points { get; private set; }
    public QuestionState State { get; private set; }
    public int Order { get; private set; }

    internal SyncedRound SyncedRound { get; }
    public ISyncedRound Round => SyncedRound;
    public Guid RoundId => SyncedRound.Id;

    internal SyncedEntitiesList<SyncedSubmittedAnswer> SyncedAnswers { get; } = new();
    public ISyncedEntities<ISyncedSubmittedAnswer> SubmittedAnswers => SyncedAnswers;

    internal SyncedQuestion(ILogger<SyncedEntity> logger, ISyncContext syncContext, QuestionDto question, SyncedRound parentRound)
        : base(logger, syncContext)
    {
        Id = question.Id;
        Text = question.Text;
        Answer = question.Answer;
        Points = question.Points;
        State = question.State;
        Order = question.Order;

        SyncedRound = parentRound;

        SyncContext.Questions.OnRevealedAsync += OnRevealedAsync;
        SyncContext.Questions.OnTextUpdatedAsync += OnTextUpdatedAsync;
        SyncContext.Questions.OnAnswerUpdatedAsync += OnAnswerUpdatedAsync;
        SyncContext.Questions.OnPointsUpdatedAsync += OnPointsUpdatedAsync;
        SyncContext.Questions.OnStateUpdatedAsync += OnStateUpdatedAsync;
        SyncContext.Questions.OnOrderUpdatedAsync += OnOrderUpdatedAsync;
    }

    public Task UpdateTextAsync(string newText) =>
        SyncContext.Questions.UpdateTextAsync(Id, newText);

    public Task UpdateAnswerAsync(string newAnswer) =>
        SyncContext.Questions.UpdateAnswerAsync(Id, newAnswer);

    public Task UpdatePointsAsync(decimal newPoints) =>
        SyncContext.Questions.UpdatePointsAsync(Id, newPoints);

    public Task UpdateStateAsync(QuestionState newState) =>
        SyncContext.Questions.UpdateStateAsync(Id, newState);

    public Task DeleteAsync() =>
        SyncContext.Questions.DeleteAsync(Id);

    private async Task OnRevealedAsync(QuestionDto question, List<SubmittedAnswerDto> answers)
    {
        if (question.Id != Id)
            return;

        Text = question.Text;
        Answer = question.Answer;
        Points = question.Points;
        State = question.State;
        Order = question.Order;

        foreach (var answer in answers)
        {
            var existingAnswer = SyncedAnswers.FirstOrDefault(syncedAnswer => syncedAnswer.Id == answer.Id);
            if (existingAnswer is null)
            {
                var participant = SyncedRound.SyncedQuiz.SyncedParticipants.FirstOrDefault(participant => participant.Id == answer.ParticipantId);
                if (participant is null)
                {
                    Logger.LogWarning("Missing participant {ParticipantId} for answer {AnswerId}.", answer.ParticipantId, answer.Id);
                    continue;
                }

                existingAnswer = new SyncedSubmittedAnswer(Logger, SyncContext, answer, this, participant);
                await SyncedAnswers.AddAsync(existingAnswer);
            }
            else
            {
                await existingAnswer.UpdateAsync(answer);
            }
        }

        await OnUpdatedAsync();
    }

    private Task OnTextUpdatedAsync(Guid id, string newText)
    {
        if (id != Id)
            return Task.CompletedTask;

        Text = newText;
        return OnUpdatedAsync();
    }

    private Task OnAnswerUpdatedAsync(Guid id, string newAnswer)
    {
        if (id != Id)
            return Task.CompletedTask;

        Answer = newAnswer;
        return OnUpdatedAsync();
    }

    private Task OnPointsUpdatedAsync(Guid id, decimal newPoints)
    {
        if (id != Id)
            return Task.CompletedTask;

        Points = newPoints;
        return OnUpdatedAsync();
    }

    private Task OnStateUpdatedAsync(Guid id, QuestionState newState)
    {
        if (id != Id)
            return Task.CompletedTask;

        State = newState;
        return OnUpdatedAsync();
    }

    private Task OnOrderUpdatedAsync(Guid id, int newOrder)
    {
        if (id != Id)
            return Task.CompletedTask;

        Order = newOrder;
        return OnUpdatedAsync();
    }

    public override int GetStateStamp() =>
        StateStamp.ForProperties(Text, Answer, Points, State, Order, SubmittedAnswers);

    public void Dispose()
    {
        SyncContext.Questions.OnRevealedAsync -= OnRevealedAsync;
        SyncContext.Questions.OnTextUpdatedAsync -= OnTextUpdatedAsync;
        SyncContext.Questions.OnAnswerUpdatedAsync -= OnAnswerUpdatedAsync;
        SyncContext.Questions.OnPointsUpdatedAsync -= OnPointsUpdatedAsync;
        SyncContext.Questions.OnStateUpdatedAsync -= OnStateUpdatedAsync;
        SyncContext.Questions.OnOrderUpdatedAsync -= OnOrderUpdatedAsync;
    }
}
