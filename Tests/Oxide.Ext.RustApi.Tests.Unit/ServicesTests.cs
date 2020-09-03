﻿using System.IO;
using System.Linq;
using NSubstitute;
using Oxide.Core;
using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Primitives.Enums;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Oxide.Ext.RustApi.Primitives.Models;
using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class ServicesTests
    {
        [Fact]
        public void AddOptions_Default_Expected()
        {
            RustApiExtension.OxideHelper = Substitute.For<IOxideHelper>();
            RustApiExtension.OxideHelper.GetInstanceDirectory().Returns(Directory.GetCurrentDirectory());

            // arrange
            var container = new MicroContainer();

            // act
            container.LoadApiOptions();
            var options = container.Get<RustApiOptions>();

            // asserts
            Assert.NotNull(options);
            Assert.Equal("http://*:28017", options.Endpoint);
            Assert.Equal(true, options.LogToFile);
            Assert.Equal(MinimumLogLevel.Debug, options.LogLevel);
        }
    }
}
