using System;
using System.Threading.Tasks;
using Quibble.Core.Entities;

namespace Quibble.UI.Core.Events
{
    public interface IQuestionEvents
    {
        public event Func<Guid, string, Task> TextUpdated;
        public event Func<Guid, string, Task> AnswerUpdated;
        public event Func<Guid, QuestionState, Task> StateUpdated;
        public event Func<IQuestion, Task> QuestionAdded;
        public event Func<Guid, Task> QuestionDeleted;
    }
}
