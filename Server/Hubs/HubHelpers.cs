using System;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.SignalR;
using Quibble.Common.Quizzes;
using Quibble.Common.Rounds;

namespace Quibble.Server.Hubs
{
    [Pure]
    internal static class HubHelpers
    {
        public static void ThrowIfNull(object? obj, string argumentName)
        {
            if (obj == null)
                throw new HubException($"{argumentName} cannot be null", new ArgumentNullException(argumentName));
        }

        public static string QuizGroupNameForQuiz(Guid quizId)
        {
            return QuizGroupNameForQuizCore(quizId);
        }

        public static string QuizGroupNameForQuiz(Quiz quiz)
        {
            if (quiz == null) throw new ArgumentNullException(nameof(quiz));
            return QuizGroupNameForQuizCore(quiz.Id);
        }

        public static string QuizGroupNameForRound(Round round)
        {
            if (round == null) throw new ArgumentNullException(nameof(round));
            return QuizGroupNameForQuizCore(round.QuizId);
        }

        private static string QuizGroupNameForQuizCore(Guid quizId) => $"quiz::{quizId}";
    }
}
