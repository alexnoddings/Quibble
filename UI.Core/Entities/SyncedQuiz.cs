using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;

namespace Quibble.UI.Core.Entities
{
    public sealed class SyncedQuiz : IQuiz, IDisposable
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; }

        public string Title { get; set; }
        public DateTime? PublishedAt { get; private set; }
        public bool IsPublished => PublishedAt != null;

        private readonly List<SyncedRound> _rounds = new();
        public IReadOnlyList<SyncedRound> Rounds => _rounds;

        private readonly List<SyncedParticipant> _participants = new();
        public IReadOnlyList<SyncedParticipant> Participants => _participants;

        private readonly AsyncEvent _updated = new();
        public event Func<Task> Updated
        {
            add => _updated.Add(value);
            remove => _updated.Remove(value);
        }

        private readonly AsyncEvent _deleted = new();
        public event Func<Task> Deleted
        {
            add => _deleted.Add(value);
            remove => _deleted.Remove(value);
        }

        private readonly AsyncEvent _published = new();
        public event Func<Task> Published
        {
            add => _published.Add(value);
            remove => _published.Remove(value);
        }

        private readonly SyncServices _services;

        internal SyncedQuiz(IQuiz dtoQuiz, IEnumerable<SyncedRound> rounds, IEnumerable<SyncedParticipant> participants, SyncServices services)
        {
            _services = services;

            Id = dtoQuiz.Id;
            OwnerId = dtoQuiz.OwnerId;
            Title = dtoQuiz.Title;
            PublishedAt = dtoQuiz.PublishedAt;
            _rounds.AddRange(rounds);
            _participants.AddRange(participants);

            _services.QuizEvents.TitleUpdated += OnTitleUpdatedAsync;
            _services.QuizEvents.Published += OnPublishedAsync;
            _services.QuizEvents.Deleted += OnDeletedAsync;

            _services.RoundEvents.RoundAdded += OnRoundAddedAsync;
            _services.RoundEvents.RoundDeleted += OnRoundDeletedAsync;

            _services.ParticipantEvents.ParticipantJoined += OnParticipantJoinedAsync;
            _services.ParticipantEvents.ParticipantLeft += OnParticipantLeftAsync;
        }

        internal SyncedQuiz(IQuiz dtoQuiz, SyncServices services)
            : this(dtoQuiz, Enumerable.Empty<SyncedRound>(), Enumerable.Empty<SyncedParticipant>(), services)
        {
        }

        public Task SaveTitleAsync() => _services.QuizService.UpdateTitleAsync(Id, Title);

        public Task PublishAsync() => _services.QuizService.PublishAsync(Id);

        public Task DeleteAsync() => _services.QuizService.DeleteAsync(Id);

        public Task AddRoundAsync() => _services.RoundService.CreateAsync(Id);

        private Task OnTitleUpdatedAsync(Guid quizId, string newTitle)
        {
            if (quizId != Id) return Task.CompletedTask;

            Title = newTitle;

            return _updated.InvokeAsync();
        }

        private Task OnPublishedAsync(Guid id, DateTime dateTime)
        {
            if (id != Id) return Task.CompletedTask;

            PublishedAt = dateTime;

            return _published.InvokeAsync();
        }

        private Task OnDeletedAsync(Guid id)
        {
            if (id != Id) return Task.CompletedTask;

            return _deleted.InvokeAsync();
        }

        private Task OnRoundAddedAsync(IRound round)
        {
            if (round.QuizId != Id) return Task.CompletedTask;

            var syncedRound = new SyncedRound(round, _services);
            _rounds.Add(syncedRound);
            return _updated.InvokeAsync();
        }

        private Task OnRoundDeletedAsync(Guid roundId)
        {
            SyncedRound? round = _rounds.FirstOrDefault(r => r.Id == roundId);
            if (round == null)
                return Task.CompletedTask;

            _rounds.Remove(round);
            return _updated.InvokeAsync();
        }

        private Task OnParticipantJoinedAsync(IParticipant participant)
        {
            if (participant.QuizId != Id) return Task.CompletedTask;

            var syncedParticipant = new SyncedParticipant(participant, _services);
            _participants.Add(syncedParticipant);
            return _updated.InvokeAsync();
        }

        private Task OnParticipantLeftAsync(Guid participantId)
        {
            SyncedParticipant? participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
                return Task.CompletedTask;

            _participants.Remove(participant);
            return _updated.InvokeAsync();
        }

        public void Dispose()
        {
            _services.QuizEvents.TitleUpdated -= OnTitleUpdatedAsync;
            _services.QuizEvents.Published -= OnPublishedAsync;
            _services.QuizEvents.Deleted -= OnDeletedAsync;

            _services.RoundEvents.RoundAdded -= OnRoundAddedAsync;
            _services.RoundEvents.RoundDeleted -= OnRoundDeletedAsync;

            _services.ParticipantEvents.ParticipantJoined -= OnParticipantJoinedAsync;
            _services.ParticipantEvents.ParticipantLeft -= OnParticipantLeftAsync;

            foreach (var round in _rounds)
                round.Dispose();
        }
    }
}
