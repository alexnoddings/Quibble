namespace Quibble.Server.Core.Sync;

public interface ISyncedUsers
{
	public ISyncedEvents All();
	public ISyncedEvents Hosts();
	public ISyncedEvents Participant(Guid participantId);
	public ISyncedEvents ParticipantExceptCaller(Guid participantId);
	public ISyncedEvents Participants();

	public Task AddCurrentUserAsHost();
	public Task AddCurrentUserAsParticipant(Guid participantId);
}
