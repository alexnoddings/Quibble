﻿using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;
using Quibble.Core.Events;
using Quibble.UI.Core.Events;

namespace Quibble.Host.Hosted.Platform.Events
{
    internal class StaticRoundEvents : IRoundEvents, IRoundEventsInvoker
    {
        public Task InvokeTitleUpdatedAsync(Guid id, string newTitle, Guid initiatorToken) => _titleUpdated.InvokeAsync(id, newTitle, initiatorToken);
        private static readonly AsyncEvent<Guid, string, Guid> _titleUpdated = new();
        public event Func<Guid, string, Guid, Task> TitleUpdated
        {
            add => _titleUpdated.Add(value);
            remove => _titleUpdated.Remove(value);
        }

        public Task InvokeStateUpdatedAsync(Guid id, RoundState newState) => _stateUpdated.InvokeAsync(id, newState);
        private static readonly AsyncEvent<Guid, RoundState> _stateUpdated = new();
        public event Func<Guid, RoundState, Task> StateUpdated
        {
            add => _stateUpdated.Add(value);
            remove => _stateUpdated.Remove(value);
        }

        public Task InvokeRoundAddedAsync(IRound newRound) => _roundAdded.InvokeAsync(newRound);
        private static readonly AsyncEvent<IRound> _roundAdded = new();
        public event Func<IRound, Task> RoundAdded
        {
            add => _roundAdded.Add(value);
            remove => _roundAdded.Remove(value);
        }

        public Task InvokeRoundDeletedAsync(Guid id) => _roundDeleted.InvokeAsync(id);
        private static readonly AsyncEvent<Guid> _roundDeleted = new();
        public event Func<Guid, Task> RoundDeleted
        {
            add => _roundDeleted.Add(value);
            remove => _roundDeleted.Remove(value);
        }
    }
}
