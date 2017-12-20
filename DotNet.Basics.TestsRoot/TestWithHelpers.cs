using System;
using System.Diagnostics;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using Xunit.Abstractions;

namespace DotNet.Basics.TestsRoot
{
    public abstract class TestWithHelpers
    {
        private static readonly DirPath _testRoot;

        static TestWithHelpers()
        {
            _testRoot = AppDomain.CurrentDomain.BaseDirectory.ToDir();
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

        protected void WithTestRoot(Action<DirPath> testRootAction)
        {
            testRootAction?.Invoke(_testRoot);
        }

        protected void ArrangeActAssertPaths(Action<DirPath> arrangeActAssert)
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            var name = method.Name;
            Output.WriteLine($"Calling class name:{name}");

            var rootDir = _testRoot.ToDir(ClassName);
            var emptyDir = _testRoot.ToDir("__Empty");
            emptyDir.CreateIfNotExists();
            
            Robocopy.Run(emptyDir.FullName(), rootDir.FullName(), "/MIR");//robust clean dir pre testing
            try
            {
                var testRootdir = rootDir.Add(TestPathPrefix);
                Output.WriteLine($"TestRootDir: {testRootdir}");
                arrangeActAssert?.Invoke(testRootdir);
            }
            finally
            {
                Robocopy.Run(emptyDir.FullName(), rootDir.FullName(), "/MIR");//robust clean dir post testing
                rootDir.DeleteIfExists();
            }
        }
    }
}
