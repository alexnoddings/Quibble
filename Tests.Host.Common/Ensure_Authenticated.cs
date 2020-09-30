using System;
using System.Collections.Generic;
using Quibble.Core.Exceptions;
using Quibble.Host.Common;
using Quibble.Host.Common.Data.Entities;
using Xunit;

namespace Quibble.Tests.Host.Common
{
    public class Ensure_Authenticated
    {
        public static IEnumerable<object?[]> Authenticated_User_Throws_Data =>
            new[]
            {
                new object?[] { null }
            };
        [Theory]
        [MemberData(nameof(Authenticated_User_Throws_Data))]
        public void Authenticated_User_Throws(DbQuibbleUser? user) =>
            Assert.Throws<UnauthenticatedException>(() => Ensure.Authenticated(user));

        public static IEnumerable<object?[]> Authenticated_User_DoesNotThrow_Data =>
            new[]
            {
                new object?[] { new DbQuibbleUser() },
                new object?[] { new DbQuibbleUser
                    {
                        Id = Guid.Parse("3986ab29-4db3-4086-9dc2-de994a1647e5"),
                        UserName = "TestUser"
                    }
                }
            };
        [Theory]
        [MemberData(nameof(Authenticated_User_DoesNotThrow_Data))]
        public void Authenticated_User_DoesNotThrow(DbQuibbleUser? user) =>
            Ensure.Authenticated(user);
    }
}
