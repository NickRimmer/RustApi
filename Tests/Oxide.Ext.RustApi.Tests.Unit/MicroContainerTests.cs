using Oxide.Ext.RustApi.Tools;
using System;
using System.Collections.Generic;
using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class MicroContainerTests
    {
        [Fact]
        public void AddGetTest_InterfaceOnly_ArgumentException()
        {
            // arrange
            var container = new MicroContainer();

            // act & assert
            Assert.Throws<ArgumentException>(() => container.Add<ITestService>());
        }


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
        public void AddGetTest_SingleInstance_Found()
        {
            // arrange
            var container = new MicroContainer();
            var expectedValue = 100;

            // act
            container.AddSingle<ITestService>(new TestService { Value = 0 });
            var service = container.Get<ITestService>();
            ((TestService)service).Value = expectedValue;

            var result = container.Get<ITestService>();

            // assert
            Assert.NotNull(service);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void AddGetTest_Single_Found()
        {
            // arrange
            var container = new MicroContainer();
            var expectedValue = 101;

            // act
            container.AddSingle<ITestService, TestService>();
            var service = container.Get<ITestService>();
            ((TestService)service).Value = expectedValue;

            var result = container.Get<ITestService>();

            // assert
            Assert.NotNull(service);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void AddGetTest_SingleInstance_NotFound()
        {
            // arrange
            var container = new MicroContainer();

            // act & assert
            container.AddSingle<TestService>(new TestService());
            Assert.Throws<KeyNotFoundException>(() => container.Get<ITestService>());
        }

        [Fact]
        public void AddGet_WithGeneric_Expected()
        {
            // arrange
            var container = new MicroContainer();

            // act
            container.Add(typeof(IWithGeneric<>), typeof(WithGeneric<>));
            var s1 = container.Get<IWithGeneric<string>>();
            var s2 = container.Get<IWithGeneric<int>>();

            // assert
            Assert.Equal(typeof(string), s1.GenericType);
            Assert.Equal(typeof(int), s2.GenericType);
        }

        [Fact]
        public void AddGet_WithGeneric_Predefined()
        {
            // arrange
            var container = new MicroContainer();

            // act
            container.Add<IWithGeneric<string>, WithGeneric<string>>();
            var s1 = container.Get<IWithGeneric<string>>();

            // assert
            Assert.Equal(typeof(string), s1.GenericType);
            Assert.Throws<KeyNotFoundException>(() => container.Get<IWithGeneric<int>>());
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

        private interface IWithGeneric<T>
        {
            Type GenericType { get; }
        }

        private class WithGeneric<T> : IWithGeneric<T>
        {
            public Type GenericType => typeof(T);
        }

        #endregion
    }
}
