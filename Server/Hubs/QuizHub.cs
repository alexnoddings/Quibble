using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Quibble.Common.SignalR;
using Quibble.Server.Models.Quizzes;

namespace Quibble.Server.Hubs
{
    /// <summary>
    /// SignalR hub for <see cref="Quiz"/> related events.
    /// </summary>
    /// <see cref="IQuizHub"/>
    [Authorize]
    public class QuizHub : Hub<IQuizHub>
    {
    }
}
