using NSubstitute;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class RustApiExtensionTests
    {
        [Fact]
        internal void ExtInfoTest_Default_Expected()
        {
            // arrange
            RustApiExtension.OxideHelper = Substitute.For<IOxideHelper>();
            var instance = new RustApiExtension(null);

            // act
            var name = instance.Name;
            var author = instance.Author;
            var version = instance.Version;

            // assert
            Assert.Equal("RustApi", name);
            Assert.Equal("Nick Rimmer", author);
            Assert.NotEqual(default, version);
        }
    }
}
