using System.Collections.Generic;

namespace Quibble.Shared.Models
{
    public class FullQuizDto
    {
        public QuizDto Quiz { get; set; }
        public List<RoundDto> Rounds { get; set; }
        public List<QuestionDto> Questions { get; set; }

        public FullQuizDto(QuizDto quiz, List<RoundDto> rounds, List<QuestionDto> questions)
        {
            Quiz = quiz;
            Rounds = rounds;
            Questions = questions;
        }
    }
}
