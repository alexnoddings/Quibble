using System.Collections.Generic;
using System.Linq;
using Quibble.Common.Rounds;

namespace Quibble.Common.Quizzes
{
    public class QuizFull : Quiz
    {
        public List<RoundFull> Rounds { get; set; } = new List<RoundFull>();

        public QuizFull()
        {

        }

        public QuizFull(Quiz other) : base(other)
        {

        }

        public QuizFull(QuizFull other) : base(other)
        {
            foreach (var round in other.Rounds)
                Rounds.Add(new RoundFull(round));
        }
    }
}
