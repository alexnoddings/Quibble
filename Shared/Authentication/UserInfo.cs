﻿using System.Collections.Generic;

namespace BlazorIdentityBase.Shared.Authentication
{
    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }

        public string UserName { get; set; } = string.Empty;

        public Dictionary<string, string> Claims { get; set; } = new();

        public static UserInfo Unauthenticated() => new() { IsAuthenticated = false };
    }
}
