using Quibble.Shared.Models.Dtos;

namespace Quibble.Shared.Models;

public class UserQuizzes
{
    public List<QuizDto> InDevelopmentQuizzes { get; set; } = new();
    public List<QuizDto> ParticipatedQuizzes { get; set; } = new();
    public List<QuizDto> HostedQuizzes { get; set; } = new();
}
