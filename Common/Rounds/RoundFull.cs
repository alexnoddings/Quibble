using System.Collections.Generic;
using Quibble.Common.Questions;

namespace Quibble.Common.Rounds
{
    public class RoundFull : Round
    {
        public List<Question> Questions { get; set; } = new List<Question>();

        public RoundFull()
        {

        }

        public RoundFull(Round other) : base(other)
        {

        }

        public RoundFull(RoundFull other) : base(other)
        {
            foreach (var question in other.Questions)
                Questions.Add(new Question(question));
        }
    }
}
