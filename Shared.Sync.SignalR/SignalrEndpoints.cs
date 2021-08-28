namespace Quibble.Shared.Sync.SignalR
{
    public static class SignalrEndpoints
    {
        public const string GetQuiz = nameof(GetQuiz);
        public const string UpdateQuizTitle = nameof(UpdateQuizTitle);
        public const string OpenQuiz = nameof(OpenQuiz);
        public const string DeleteQuiz = nameof(DeleteQuiz);

        public const string CreateRound = nameof(CreateRound);
        public const string UpdateRoundTitle = nameof(UpdateRoundTitle);
        public const string OpenRound = nameof(OpenRound);
        public const string DeleteRound = nameof(DeleteRound);

        public const string CreateQuestion = nameof(CreateQuestion);
        public const string UpdateQuestionText = nameof(UpdateQuestionText);
        public const string UpdateQuestionAnswer = nameof(UpdateQuestionAnswer);
        public const string UpdateQuestionPoints = nameof(UpdateQuestionPoints);
        public const string UpdateQuestionState = nameof(UpdateQuestionState);
        public const string DeleteQuestion = nameof(DeleteQuestion);

        public const string PreviewUpdateSubmittedAnswerText = nameof(PreviewUpdateSubmittedAnswerText);
        public const string UpdateSubmittedAnswerText = nameof(UpdateSubmittedAnswerText);
        public const string UpdateSubmittedAnswerAssignedPoints = nameof(UpdateSubmittedAnswerAssignedPoints);
    }
}
