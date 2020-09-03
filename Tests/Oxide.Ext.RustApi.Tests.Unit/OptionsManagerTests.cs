using System.IO;
using NSubstitute;
using Oxide.Ext.RustApi.Business.Common;
using Oxide.Ext.RustApi.Business.Services;
using Oxide.Ext.RustApi.Primitives.Interfaces;
using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class OptionsManagerTests
    {
        [Fact]
        public void ReadOptions_NotExist_CreatedNew()
        {
            // arrange
            var data = new
            {
                Option1 = true,
                Option2 = "second"
            };

            var container = Substitute.For<MicroContainer>();
            RustApiExtension.OxideHelper = Substitute.For<IOxideHelper>();

            var fileName = "test1-config.json";
            
            // act
            OptionsManager.WriteOptions(fileName, data, container);

            var fileExists = File.Exists(fileName);
            var content = fileExists ? File.ReadAllText(fileName) : null;

            // assert
            Assert.True(fileExists);
            Assert.NotNull(content);
            Assert.Contains("Option1", content);
            Assert.Contains("Option2", content);
            Assert.Contains("second", content);
        }
    }
}
