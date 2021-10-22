using Microsoft.Extensions.Logging;
using Quibble.Client.Sync.Core.Contexts;
using Quibble.Client.Sync.Core.Entities;
using Quibble.Shared.Entities;
using Quibble.Shared.Models.Dtos;

namespace Quibble.Client.Sync.Entities;

internal sealed class SyncedSubmittedAnswer : SyncedEntity, ISyncedSubmittedAnswer, IDisposable
{
    public string Text { get; private set; }
    public decimal AssignedPoints { get; private set; }

    internal SyncedQuestion SyncedQuestion { get; }
    public ISyncedQuestion Question => SyncedQuestion;
    public Guid QuestionId => SyncedQuestion.Id;

    internal SyncedParticipant SyncedParticipant { get; }
    public ISyncedParticipant Submitter => SyncedParticipant;
    public Guid ParticipantId => SyncedParticipant.Id;

    internal SyncedSubmittedAnswer(ILogger<SyncedEntity> logger, ISyncContext syncContext, SubmittedAnswerDto submittedAnswer, SyncedQuestion syncedQuestion, SyncedParticipant syncedParticipant)
        : base(logger, syncContext)
    {
        Id = submittedAnswer.Id;
        Text = submittedAnswer.Text;
        AssignedPoints = submittedAnswer.AssignedPoints;

        SyncedQuestion = syncedQuestion;
        SyncedParticipant = syncedParticipant;

        SyncContext.SubmittedAnswers.OnAssignedPointsUpdatedAsync += OnAssignedPointsUpdatedAsync;
        SyncContext.SubmittedAnswers.OnTextUpdatedAsync += OnTextUpdatedAsync;
    }

    public Task PreviewUpdateTextAsync(string previewText) =>
        SyncContext.SubmittedAnswers.PreviewUpdateTextAsync(Id, previewText);

    public Task UpdateTextAsync(string newText) =>
        SyncContext.SubmittedAnswers.UpdateTextAsync(Id, newText);

    public Task MarkAsync(decimal points) =>
        SyncContext.SubmittedAnswers.MarkAsync(Id, points);

    internal Task UpdateAsync(ISubmittedAnswer submittedAnswer)
    {
        Text = submittedAnswer.Text;
        AssignedPoints = submittedAnswer.AssignedPoints;

        return OnUpdatedAsync();
    }

    private Task OnAssignedPointsUpdatedAsync(Guid id, decimal newPoints)
    {
        if (id != Id)
            return Task.CompletedTask;

        AssignedPoints = newPoints;
        return OnUpdatedAsync();
    }

    private Task OnTextUpdatedAsync(Guid id, string newText)
    {
        if (id != Id)
            return Task.CompletedTask;

        Text = newText;
        return OnUpdatedAsync();
    }

    public override int GetStateStamp() =>
        StateStamp.ForProperties(Text, AssignedPoints);

    public void Dispose()
    {
        SyncContext.SubmittedAnswers.OnAssignedPointsUpdatedAsync -= OnAssignedPointsUpdatedAsync;
        SyncContext.SubmittedAnswers.OnTextUpdatedAsync -= OnTextUpdatedAsync;
    }
}
