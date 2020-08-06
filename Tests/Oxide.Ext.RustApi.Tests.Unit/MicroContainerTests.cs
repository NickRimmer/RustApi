using System;
using System.Collections.Generic;
using Oxide.Ext.RustApi.Tools;
using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class MicroContainerTests
    {
        [Fact]
        public void AddGetTest_ByInterface_Found()
        {
            // arrange
            var container = new MicroContainer();

            // act
            container.Add<ITestService, TestService>();
            var result = container.Get<ITestService>();

            // assert
            Assert.NotNull(result);
            Assert.Equal(TestService.DefaultValue, result.Value);
        }

        [Fact]
        public void AddGetTest_ByInterface_NotFound()
        {
            // arrange
            var container = new MicroContainer();

            // act & assert
            container.Add<ITestService, TestService>();
            Assert.Throws<KeyNotFoundException>(() => container.Get<TestService>());
        }

        [Fact]
        public void AddGetTest_ByImplementation_Found()
        {
            // arrange
            var container = new MicroContainer();

            // act
            container.Add<TestService>();
            var result = container.Get<TestService>();

            // assert
            Assert.NotNull(result);
            Assert.Equal(TestService.DefaultValue, result.Value);
        }

        [Fact]
        public void AddGetTest_ByImplementation_NotFound()
        {
            // arrange
            var container = new MicroContainer();

            // act & assert
            container.Add<TestService>();
            Assert.Throws<KeyNotFoundException>(() => container.Get<ITestService>());
        }

        [Fact]
        public void AddGetTest_Instance_Found()
        {
            // arrange
            var container = new MicroContainer();
            var expectedValue = 100;

            // act
            container.Add<ITestService>(new TestService { Value = expectedValue });
            var result = container.Get<ITestService>();

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void AddGetTest_Instance_NotFound()
        {
            // arrange
            var container = new MicroContainer();

            // act & assert
            container.Add<TestService>(new TestService());
            Assert.Throws<KeyNotFoundException>(() => container.Get<ITestService>());
        }

        [Fact]
        public void InjectionTest_Default_Found()
        {
            // arrange
            var container = new MicroContainer();

            // act
            container.Add<TestDepService>();
            container.Add<ITestService, TestService>();

            var result = container.Get<TestDepService>();

            // assert
            Assert.NotNull(result);
            Assert.NotNull(result.SomeServiceInstance);
            Assert.Equal(TestService.DefaultValue, result.SomeServiceInstance.Value);
        }

        [Fact]
        public void InjectionTest_Default_NotFound()
        {
            // arrange
            var container = new MicroContainer();

            // act & assert
            container.Add<TestDepService>();

            var ex = Assert.Throws<KeyNotFoundException>(() => container.Get<TestDepService>());
            Assert.Equal(typeof(ITestService).FullName, ex.Message);
        }

        #region Test service interface and implementations

        private interface ITestService
        {
            int Value { get; }
        }

        private class TestService : ITestService
        {
            public static int DefaultValue = 10;
            public int Value { get; set; } = DefaultValue;
        }

        private class TestDepService
        {
            public readonly ITestService SomeServiceInstance;

            public TestDepService(ITestService someService)
            {
                SomeServiceInstance = someService ?? throw new ArgumentNullException(nameof(someService));
            }
        }

        #endregion
    }
}
