using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests
{
    public abstract class TestWithHelpers
    {
        private static readonly DirPath _testRoot;
        static TestWithHelpers()
        {
            _testRoot = AppDomain.CurrentDomain.BaseDirectory.EnsurePrefix("/").ToDir();
        }

        protected TestWithHelpers(ITestOutputHelper output, string testPathPrefix = null)
        {
            ClassName = GetType().Name;
            TestPathPrefix = testPathPrefix ?? string.Empty;
            Output = output;
        }

        protected string ClassName { get; }
        protected string TestPathPrefix { get; }
        protected ITestOutputHelper Output { get; }

        protected IServiceProvider GetEmptyServiceProvider()
        {
            return new ServiceCollection().BuildServiceProvider();
        }
        protected IServiceProvider GetTransientServiceProvider(Type t)
        {
            return new ServiceCollection().AddTransient(t).BuildServiceProvider();
        }
        protected IServiceProvider GetTransientServiceProvider<T>() where T : class
        {
            return new ServiceCollection().AddTransient<T>().BuildServiceProvider();
        }
        protected IServiceProvider GetSingletonServiceProvider<T>() where T : class
        {
            return new ServiceCollection().AddSingleton<T>().BuildServiceProvider();
        }

        protected void WithTestRoot(Action<DirPath> testRootAction)
        {
            testRootAction?.Invoke(_testRoot);
        }

        protected Task WithTestRootAsync(Func<DirPath, Task> testRootAction)
        {
            return testRootAction?.Invoke(_testRoot);
        }

        protected void ArrangeActAssertPaths(Action<DirPath> arrangeActAssert)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            var name = method.Name;
            Output.WriteLine($"Calling class name:{name}");

            var rootDir = _testRoot.ToDir(ClassName);
            try
            {
                var testRootDir = rootDir.Add(TestPathPrefix);
                testRootDir.CreateIfNotExists();
                testRootDir.CleanIfExists();
                Output.WriteLine($"TestRootDir: {testRootDir}");
                arrangeActAssert?.Invoke(testRootDir);
            }
            finally
            {
                rootDir.DeleteIfExists();
            }
        }
    }
}