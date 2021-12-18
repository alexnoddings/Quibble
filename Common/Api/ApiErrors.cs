namespace Quibble.Common.Api;

public static class ApiErrors
{
	public static ApiError Unauthorised { get; } = new(401, nameof(Unauthorised));
	public static ApiError UnknownError { get; } = new(500, nameof(UnknownError));

	public static ApiError CantDeleteAsNotInDevelopment { get; } = new(400, nameof(CantDeleteAsNotInDevelopment));
	public static ApiError CantUpdateAsNotOpen { get; } = new(400, nameof(CantUpdateAsNotOpen));
	public static ApiError CantEditAsNotOwner { get; } = new(403, nameof(CantEditAsNotOwner));
	public static ApiError TextTooLong { get; } = new(400, nameof(TextTooLong));

	public static ApiError QuizAlreadyOpen { get; } = new(400, nameof(QuizAlreadyOpen));
	public static ApiError QuizEmpty { get; } = new(400, nameof(QuizEmpty));
	public static ApiError QuizNotFound { get; } = new(404, nameof(QuizNotFound));
	public static ApiError QuizNotOpen { get; } = new(403, nameof(QuizNotOpen));
	public static ApiError QuizUserNotParticipant { get; } = new(403, nameof(QuizUserNotParticipant));

	public static ApiError RoundTitleTooLong { get; } = new(400, nameof(RoundTitleTooLong));
	public static ApiError RoundMissingTitle { get; } = new(400, nameof(RoundMissingTitle));
	public static ApiError RoundNotFound { get; } = new(404, nameof(RoundNotFound));
	public static ApiError RoundParentQuizNotFound { get; } = new(404, nameof(RoundParentQuizNotFound));

	public static ApiError PointsTooLow { get; } = new(400, nameof(PointsTooLow));
	public static ApiError PointsTooHigh { get; } = new(400, nameof(PointsTooHigh));

	public static ApiError QuestionBadState { get; } = new(400, nameof(QuestionBadState));
	public static ApiError QuestionMissingText { get; } = new(400, nameof(QuestionMissingText));
	public static ApiError QuestionMissingAnswer { get; } = new(400, nameof(QuestionMissingAnswer));
	public static ApiError QuestionNotFound { get; } = new(404, nameof(QuestionNotFound));
	public static ApiError QuestionParentRoundNotFound { get; } = new(404, nameof(QuestionParentRoundNotFound));

	public static ApiError AnswerNotFound { get; } = new(403, nameof(AnswerNotFound));
}
