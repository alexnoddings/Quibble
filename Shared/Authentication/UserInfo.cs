using System.Collections.Generic;

namespace Quibble.Shared.Authentication
{
    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }
        public string AuthenticationType { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public Dictionary<string, string> Claims { get; set; } = new();

        public static UserInfo Unauthenticated() => new() { IsAuthenticated = false };
    }
}
