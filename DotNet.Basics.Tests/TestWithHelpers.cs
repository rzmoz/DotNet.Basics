﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
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
            var method = frame.GetMethod()!;
            var name = method.Name;
            Output.WriteLine($"Calling class name:{name}");

            var rootDir = _testRoot.ToDir(ClassName);
            try
            {
                var testRootDir = rootDir.Add(TestPathPrefix);
                testRootDir.CreateIfNotExists();
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