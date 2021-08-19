using System.Text.Json.Serialization;

namespace Quibble.Shared.Models.Dtos
{
    public class FullQuizDto
    {
        public QuizDto Quiz { get; set; }
        public List<ParticipantDto> Participants { get; }
        public List<RoundDto> Rounds { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public List<SubmittedAnswerDto> SubmittedAnswers { get; }

        public FullQuizDto(QuizDto quiz, List<RoundDto> rounds, List<QuestionDto> questions)
        {
            Quiz = quiz;
            Participants = new List<ParticipantDto>();
            Rounds = rounds;
            Questions = questions;
            SubmittedAnswers = new List<SubmittedAnswerDto>();
        }

        [JsonConstructor]
        public FullQuizDto(QuizDto quiz, List<ParticipantDto> participants, List<RoundDto> rounds, List<QuestionDto> questions, List<SubmittedAnswerDto> submittedAnswers)
        {
            Quiz = quiz;
            Participants = participants;
            Rounds = rounds;
            Questions = questions;
            SubmittedAnswers = submittedAnswers;
        }
    }
}
