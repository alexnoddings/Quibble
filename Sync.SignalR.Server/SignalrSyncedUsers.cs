using Microsoft.AspNetCore.SignalR;
using Quibble.Server.Core.Sync;

namespace Quibble.Sync.SignalR.Server;

internal class SignalrSyncedUsers : ISyncedUsers
{
	private Guid QuizId { get; }
	private Hub<ISyncedEvents> Hub { get; }

	public SignalrSyncedUsers(Guid quizId, Hub<ISyncedEvents> hub)
	{
		QuizId = quizId;
		Hub = hub;
	}

	public ISyncedEvents All() => Hub.Clients.Group($"quiz::{QuizId}::all");
	public ISyncedEvents Hosts() => Hub.Clients.Group($"quiz::{QuizId}::hosts");
	public ISyncedEvents Participant(Guid participantId) => Hub.Clients.Group($"quiz::{QuizId}::{participantId}");
	public ISyncedEvents ParticipantExceptCaller(Guid participantId) => Hub.Clients.OthersInGroup($"quiz::{QuizId}::{participantId}");
	public ISyncedEvents Participants() => Hub.Clients.Group($"quiz::{QuizId}::participants");

	public async Task AddCurrentUserAsHost()
	{
		await Hub.Groups.AddToGroupAsync(Hub.Context.ConnectionId, $"quiz::{QuizId}::all");
		await Hub.Groups.AddToGroupAsync(Hub.Context.ConnectionId, $"quiz::{QuizId}::hosts");
	}

	public async Task AddCurrentUserAsParticipant(Guid participantId)
	{
		await Hub.Groups.AddToGroupAsync(Hub.Context.ConnectionId, $"quiz::{QuizId}::all");
		await Hub.Groups.AddToGroupAsync(Hub.Context.ConnectionId, $"quiz::{QuizId}::{participantId}");
		await Hub.Groups.AddToGroupAsync(Hub.Context.ConnectionId, $"quiz::{QuizId}::participants");
	}
}
