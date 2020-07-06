using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Quibble.Common.SignalR;

namespace Quibble.Server.Hubs
{
    [Authorize]
    public class QuizHub : Hub<IQuizHub>
    {
    }
}
