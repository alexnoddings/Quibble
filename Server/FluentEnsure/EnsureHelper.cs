using Quibble.Server.Data.Models;

namespace Quibble.Server.FluentEnsure
{
    public static class EnsureHelper
    {
        public static Ensurety<object> Exists =>
            new((entity, _) => entity is not null, "Entity does not exist.");

        public static Ensurety<DbQuiz> UserOwnsQuiz =>
            new((quiz, userId) => quiz.OwnerId == userId, "You do not own this quiz.");
    }
}
