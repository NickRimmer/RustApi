﻿using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class RustApiEntryTests
    {
        [Fact]
        public void ExtInfoTest_Default_Expected()
        {
            // arrange
            var instance = new RustApiEntry(null);

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