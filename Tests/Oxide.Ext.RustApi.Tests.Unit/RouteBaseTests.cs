using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Models;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class RouteBaseTests
    {
        [Theory]
        [InlineData(true, "admin", "something", "else", "here")] // cause admin (;
        [InlineData(true, "feature-1", "feature-2", "buzinga", "feature-1")] // feature-1 in both
        [InlineData(true, "feature-1", "feature-2", null, null)] // public method without required permissions
        [InlineData(false, "feature-1", "feature-2", "feature-3", "feature-4")] // user not admin no one required permission has
        [InlineData(false, "feature-1", "feature-2", "feature-3", "admin")] // user not admin no one required permission has
        public void IsUserHasAccess_Default_Expected(bool expectedResult, string userP1, string userP2, string reqP1, string reqP2)
        {
            // arrange
            var userPermissions = new[] { userP1, userP2 }.Where(x => !string.IsNullOrEmpty(x)).ToList();
            var requiredPermissions = new[] {reqP1, reqP2}.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var user = new ApiUserInfo("TestUser", "SuperSecret", userPermissions);

            // act
            var method = typeof(TestRoute).GetMethod("IsUserHasAccess", BindingFlags.Instance | BindingFlags.NonPublic);
            var result = method?.Invoke(new TestRoute(), new object[] { user, requiredPermissions }) as bool?;

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult, result.Value);
        }

        private class TestRoute : RouteBase
        {

        }
    }
}
