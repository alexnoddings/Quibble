namespace Quibble.Common.Dtos;

public class UserQuizzesDto
{
	public List<QuizDto> InDevelopmentQuizzes { get; set; } = new();
	public List<QuizDto> ParticipatedQuizzes { get; set; } = new();
	public List<QuizDto> HostedQuizzes { get; set; } = new();
}
