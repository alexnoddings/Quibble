﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;

namespace Quibble.UI.Core.Entities
{
    public class SyncedQuiz : IQuiz, IDisposable
    {
        private Guid Token { get; } = Guid.NewGuid();

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
        private bool _isDisposed;

        internal SyncedQuiz(IQuiz dtoQuiz, IEnumerable<SyncedRound> rounds, IEnumerable<SyncedParticipant> participants, SyncServices services)
        {
            _services = services;

            Id = dtoQuiz.Id;
            OwnerId = dtoQuiz.OwnerId;
            Title = dtoQuiz.Title;
            PublishedAt = dtoQuiz.PublishedAt;

            _rounds.AddRange(rounds);
            foreach (var round in _rounds)
                round.Updated += OnRoundUpdatedInternalAsync;

            _participants.AddRange(participants);
            foreach (var participant in _participants)
                participant.Updated += OnParticipantUpdatedInternalAsync;

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

        public Task SaveTitleAsync() => _services.QuizService.UpdateTitleAsync(Id, Title, Token);

        public Task PublishAsync() => _services.QuizService.PublishAsync(Id);

        public Task DeleteAsync() => _services.QuizService.DeleteAsync(Id);

        public Task AddRoundAsync() => _services.RoundService.CreateAsync(Id);

        private Task OnTitleUpdatedAsync(Guid quizId, string newTitle, Guid initiatorToken)
        {
            if (Token == initiatorToken) return Task.CompletedTask;
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
            syncedRound.Updated += OnRoundUpdatedInternalAsync;
            _rounds.Add(syncedRound);
            return _updated.InvokeAsync();
        }

        private Task OnRoundDeletedAsync(Guid roundId)
        {
            SyncedRound? round = _rounds.FirstOrDefault(r => r.Id == roundId);
            if (round == null)
                return Task.CompletedTask;

            round.Updated -= OnRoundUpdatedInternalAsync;
            _rounds.Remove(round);
            return _updated.InvokeAsync();
        }

        private Task OnRoundUpdatedInternalAsync() => _updated.InvokeAsync();

        private Task OnParticipantJoinedAsync(IParticipant participant)
        {
            if (participant.QuizId != Id) return Task.CompletedTask;

            var syncedParticipant = new SyncedParticipant(participant, _services);
            syncedParticipant.Updated += OnParticipantUpdatedInternalAsync;
            _participants.Add(syncedParticipant);
            return _updated.InvokeAsync();
        }

        private Task OnParticipantLeftAsync(Guid participantId)
        {
            SyncedParticipant? participant = _participants.FirstOrDefault(p => p.Id == participantId);
            if (participant == null)
                return Task.CompletedTask;

            participant.Updated -= OnParticipantUpdatedInternalAsync;
            _participants.Remove(participant);
            return _updated.InvokeAsync();
        }

        private Task OnParticipantUpdatedInternalAsync() => _updated.InvokeAsync();

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _services.QuizEvents.TitleUpdated -= OnTitleUpdatedAsync;
                    _services.QuizEvents.Published -= OnPublishedAsync;
                    _services.QuizEvents.Deleted -= OnDeletedAsync;

                    _services.RoundEvents.RoundAdded -= OnRoundAddedAsync;
                    _services.RoundEvents.RoundDeleted -= OnRoundDeletedAsync;

                    _services.ParticipantEvents.ParticipantJoined -= OnParticipantJoinedAsync;
                    _services.ParticipantEvents.ParticipantLeft -= OnParticipantLeftAsync;

                    foreach (var round in _rounds)
                    {
                        round.Updated -= OnRoundUpdatedInternalAsync;
                        round.Dispose();
                    }

                    foreach (var participant in _participants)
                    {
                        participant.Updated -= OnParticipantUpdatedInternalAsync;
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
