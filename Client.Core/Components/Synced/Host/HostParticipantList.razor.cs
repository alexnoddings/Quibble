using Microsoft.AspNetCore.Components;
using Quibble.Client.Core.Entities;

namespace Quibble.Client.Core.Components.Synced.Host;

public sealed partial class HostParticipantList : IDisposable
{
	[Parameter]
	public ISyncedQuiz Quiz { get; set; } = default!;

	private decimal TotalQuestionPoints { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();

		// This won't change for the quiz lifetime
		TotalQuestionPoints = Quiz.Rounds.SelectMany(r => r.Questions).Sum(q => q.Points);

		Quiz.Participants.Added += OnParticipantJoinedAsync;

		foreach (var participant in Quiz.Participants)
			foreach (var answer in participant.Answers)
				answer.Updated += OnUpdatedAsync;
	}

	private Task OnParticipantJoinedAsync(ISyncedParticipant participant)
	{
		foreach (var answer in participant.Answers)
			answer.Updated += OnUpdatedAsync;

		return InvokeAsync(StateHasChanged);
	}

	private IEnumerable<(decimal, List<ISyncedParticipant>)> GetParticipantScores() =>
		from participant in Quiz.Participants
		let score = participant
			.Answers
			.Select(answer => answer.AssignedPoints)
			.Where(assignedPoints => assignedPoints >= 0)
			.Sum()
		group participant by score
		into groupedParticipants
		orderby groupedParticipants.Key descending
		select (groupedParticipants.Key, groupedParticipants.ToList());

	protected override int CalculateStateStamp() =>
		StateStamp.ForProperties(Quiz.Participants);

	public void Dispose()
	{
		foreach (var participant in Quiz.Participants)
			foreach (var answer in participant.Answers)
				answer.Updated -= OnUpdatedAsync;
	}
}
