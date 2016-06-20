using System;
using System.Linq;
using System.Management.Automation;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellConsoleTests
    {
        [Test]
        public void RunScript_ExecuteScript_HelloworldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellConsole.RunScript($"\"{greeting}\"");

            result.Single().ToString().Should().Be(greeting);
        }

        [Test]
        public void MoveItem_Folder_FolderIsMoved()
        {
            var dummyContent = "Lorem Ipsum sdfgjnsndfkjsdfkhsdfjksdfsdf";

            var sourceDir = TestContext.CurrentContext.TestDirectory.ToDir("MoveItem_Folder_FolderIsMoved");
            var sourceFile = sourceDir.ToFile("Myfile.txt");
            var targetDir = TestContext.CurrentContext.TestDirectory.ToDir("MoveItem_Folder_FolderIsMoved_Target");
            var targetFile = targetDir.ToFile(sourceFile.Name);

            targetDir.DeleteIfExists();
            sourceDir.CreateIfNotExists();
            sourceDir.CleanIfExists();
            dummyContent.WriteAllText(sourceFile);

            targetFile.Exists().Should().BeFalse();
            targetDir.Exists().Should().BeFalse();

            //act
            PowerShellConsole.MoveItem(sourceDir.FullName, targetDir.FullName, true);

            //assert
            sourceDir.Exists().Should().BeFalse();
            targetDir.Exists().Should().BeTrue();
            targetFile.Exists().Should().BeTrue();
            targetFile.ReadAllText().Should().Be(dummyContent);
        }

        [Test]
        public void MoveItem_SourceNotFound_ExceptionIsThrown()
        {
            var sourceDir = TestContext.CurrentContext.TestDirectory.ToDir("SOMETHINGTHATDOESNTEXIST");
            var targetDir = TestContext.CurrentContext.TestDirectory.ToDir($"{sourceDir.Name}EITHER");
            sourceDir.DeleteIfExists();

            //act
            Action action = () => PowerShellConsole.MoveItem(sourceDir.FullName, targetDir.FullName, true);

            //assert
            action.ShouldThrow<ItemNotFoundException>();
        }
    }
}
