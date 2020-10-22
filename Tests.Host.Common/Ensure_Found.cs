using System;
using System.Collections.Generic;
using Quibble.Core.Exceptions;
using Quibble.Host.Common;
using Xunit;

namespace Quibble.Tests.Host.Common
{
    public class Ensure_Found
    {
        public static IEnumerable<object?[]> Found_SimpleEntity_Throws_Data =>
            new[]
            {
                new object?[] { null }
            };
        [Theory]
        [MemberData(nameof(Found_SimpleEntity_Throws_Data))]
        public void Found_Object_Throws(SimpleEntity? entity) =>
            Assert.Throws<NotFoundException>(() => Ensure.Found(entity, "SimpleEntity", entity?.Id ?? Guid.Empty));

        public static IEnumerable<object?[]> Found_SimpleEntity_DoesNotThrow_Data =>
            new[]
            {
                new object?[] { new SimpleEntity() }
            };
        [Theory]
        [MemberData(nameof(Found_SimpleEntity_DoesNotThrow_Data))]
        public void Found_Object_DoesNotThrow(SimpleEntity? entity) =>
            Ensure.Found(entity, "SimpleEntity", entity?.Id ?? Guid.Empty);
    }
}
