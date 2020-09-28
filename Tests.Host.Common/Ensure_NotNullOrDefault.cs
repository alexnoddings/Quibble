using System;
using System.Collections.Generic;
using Quibble.Host.Common;
using Xunit;

namespace Quibble.Tests.Host.Common
{
    public class Ensure_NotNullOrDefault
    {
        public static IEnumerable<object?[]> NotNullOrDefault_Int_Throws_Data =>
            new[]
            {
                new object?[] { default(int) }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_Int_Throws_Data))]
        public void NotNullOrDefault_Int_Throws(int @int) =>
            Assert.Throws<ArgumentException>(() => Ensure.NotNullOrDefault(@int, nameof(@int)));

        public static IEnumerable<object?[]> NotNullOrDefault_Guid_Throws_Data =>
            new[]
            {
                new object?[] { default(Guid) }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_Guid_Throws_Data))]
        public void NotNullOrDefault_Guid_Throws(Guid guid) =>
            Assert.Throws<ArgumentException>(() => Ensure.NotNullOrDefault(guid, nameof(guid)));

        public static IEnumerable<object?[]> NotNullOrDefault_String_Throws_Data =>
            new[]
            {
                new object?[] { default(string) }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_String_Throws_Data))]
        public void NotNullOrDefault_String_Throws(string str) =>
            Assert.Throws<ArgumentNullException>(() => Ensure.NotNullOrDefault(str, nameof(str)));

        public static IEnumerable<object?[]> NotNullOrDefault_Object_Throws_Data =>
            new[]
            {
                new object?[] { null }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_Object_Throws_Data))]
        public void NotNullOrDefault_Object_Throws(SimpleObject? obj) =>
            Assert.Throws<ArgumentNullException>(() => Ensure.NotNullOrDefault(obj, nameof(obj)));

        public static IEnumerable<object?[]> NotNullOrDefault_Int_DoesNotThrow_Data =>
            new[]
            {
                new object?[] { -1 },
                new object?[] { 1 },
                new object?[] { int.MinValue },
                new object?[] { int.MaxValue }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_Int_DoesNotThrow_Data))]
        public void NotNullOrDefault_Int_DoesNotThrow(int @int) =>
            Ensure.NotNullOrDefault(@int, nameof(@int));

        public static IEnumerable<object?[]> NotNullOrDefault_Guid_DoesNotThrow_Data =>
            new[]
            {
                new object?[] { Guid.Parse("c473438d-be27-48f0-ba51-a73e1e5cbf29") },
                new object?[] { Guid.Parse("79a8887d-f913-403e-a1d4-aa411d0e932b") },
                new object?[] { Guid.Parse("10000000-0000-0000-0000-000000000000") },
                new object?[] { Guid.Parse("00000000-0000-0000-0000-00000000000a") }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_Guid_DoesNotThrow_Data))]
        public void NotNullOrDefault_Guid_DoesNotThrow(Guid guid) =>
            Ensure.NotNullOrDefault(guid, nameof(guid));

        public static IEnumerable<object?[]> NotNullOrDefault_String_DoesNotThrow_Data =>
            new[]
            {
                new object?[] { string.Empty },
                new object?[] { "string" },
                new object?[] { "   " }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_String_DoesNotThrow_Data))]
        public void NotNullOrDefault_String_DoesNotThrow(string str) =>
            Ensure.NotNullOrDefault(str, nameof(str));

        public static IEnumerable<object?[]> NotNullOrDefault_Object_DoesNotThrow_Data =>
            new[]
            {
                new object?[] { new SimpleObject() }
            };
        [Theory]
        [MemberData(nameof(NotNullOrDefault_Object_DoesNotThrow_Data))]
        public void NotNullOrDefault_Object_DoesNotThrow(SimpleObject? obj) =>
            Ensure.NotNullOrDefault(obj, nameof(obj));
    }
}
