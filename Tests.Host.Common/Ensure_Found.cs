using System.Collections.Generic;
using Quibble.Core.Exceptions;
using Quibble.Host.Common;
using Xunit;

namespace Quibble.Tests.Host.Common
{
    public class Ensure_Found
    {
        public static IEnumerable<object?[]> Found_Object_Throws_Data =>
            new[]
            {
                new object?[] { null }
            };
        [Theory]
        [MemberData(nameof(Found_Object_Throws_Data))]
        public void Found_Object_Throws(object? obj) =>
            Assert.Throws<NotFoundException>(() => Ensure.Found(obj, "object"));

        public static IEnumerable<object?[]> Found_Object_DoesNotThrow_Data =>
            new[]
            {
                new object?[] { "    " },
                new object?[] { new SimpleObject() }
            };
        [Theory]
        [MemberData(nameof(Found_Object_DoesNotThrow_Data))]
        public void Found_Object_DoesNotThrow(object? obj) =>
            Ensure.Found(obj, "object");
    }
}
