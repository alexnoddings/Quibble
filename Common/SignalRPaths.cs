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
        /// The path for the Quiz hub
        /// </summary>
        public const string QuizHub = HubsBase + "/quiz";
    }
}
