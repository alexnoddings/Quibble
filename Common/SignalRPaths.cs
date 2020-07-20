namespace Quibble.Common
{
    /// <summary>
    /// Provides a central location to co-ordinate SignalR hub paths.
    /// </summary>
    public static class SignalRPaths
    {
        /// <summary>
        /// The base path for hubs.
        /// </summary>
        public const string HubsBase = "/hubs";

        /// <summary>
        /// The path for the Quiz hub.
        /// </summary>
        public const string QuizHub = HubsBase + "/quiz";

        /// <summary>
        /// The path for the Round hub.
        /// </summary>
        public const string RoundHub = HubsBase + "/round";

        /// <summary>
        /// The path for the Question hub.
        /// </summary>
        public const string QuestionHub = HubsBase + "/question";
    }
}
