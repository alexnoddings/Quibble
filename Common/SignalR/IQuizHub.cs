using System.Threading.Tasks;

namespace Quibble.Common.SignalR
{
    public interface IQuizHub
    {
        Task OnQuizTitleUpdated(string newTitle);
    }
}
