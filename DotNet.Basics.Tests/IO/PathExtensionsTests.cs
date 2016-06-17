﻿using System;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathExtensionsTests
    {
        [Test]
        [TestCase("Exists_TestPath_PathIsVerified", true)]//dir
        [TestCase("Exists_TestPath_PathIsVerified\\file.txt", false)]//file
        public void Exists_TestPath_PathIsVerified(string path, bool isFolder)
        {
            Path p = null;
            if (isFolder)
                p = path.ToDir();
            else
                p = path.ToFile();

            p.DeleteIfExists();
            p.Exists().Should().BeFalse(p.FullName);

            if (p.IsFolder)
                p.CreateIfNotExists();
            else
                "dummycontent".WriteAllText(p);

            p.Exists().Should().BeTrue(p.FullName);
        }

        [Test]
        [TestCase(PathDelimiter.Slash, '/')]
        [TestCase(PathDelimiter.Backslash, '\\')]
        public void ToChar_DelimiterToChar_DelimiterIsConverted(PathDelimiter delimiter, char expectedChar)
        {
            var foundChar = delimiter.ToChar();
            foundChar.Should().Be(expectedChar);
        }

        [Test]
        [TestCase('/', PathDelimiter.Slash)]
        [TestCase('\\', PathDelimiter.Backslash)]
        public void ToPathDelimiter_CharToDelimiter_CharIsConverted(char delimiter, PathDelimiter expectedPathDelimiter)
        {
            var foundPathDelimiter = delimiter.ToPathDelimiter();
            foundPathDelimiter.Should().Be(expectedPathDelimiter);
        }

        [Test]
        [TestCase('a')]
        [TestCase('1')]
        [TestCase('¤')]
        public void ToPathDelimiter_UnsupportedChar_ExceptionIsThrown(char delimiter)
        {
            Action action = () => delimiter.ToPathDelimiter();
            action.ShouldThrow<NotSupportedException>();
        }

        [Test]
        [TestCase("//pt101", "pt2", PathDelimiter.Slash)]//file
        [TestCase("\\pt101", "pt2", PathDelimiter.Backslash)]//file
        [TestCase("//pt101", "pt2/", PathDelimiter.Slash)]//dir
        [TestCase("\\pt101", "pt2\\", PathDelimiter.Backslash)]//dir
        public void ToPath_Combine_PathIsGenerated(string pt1, string pt2, PathDelimiter pathDelimiter)
        {
            var path = pt1.ToPath(pt2);

            var refPath = pt1 + pathDelimiter.ToChar() + pt2;
            if (path.IsFolder == false)
                refPath = refPath.TrimEnd(pathDelimiter.ToChar());
            refPath = refPath.TrimStart(pathDelimiter.ToChar());
            path.RawName.Should().Be(refPath);
        }

        [Test]
        [TestCase("mypath", false)]//file
        [TestCase("mypath", true)]//file
        public void IsFolder_Set_IsFolderIsSet(string pth, bool isFolder)
        {
            Path p;
            if (isFolder)
                p = pth.ToDir();
            else
                p = pth.ToFile();

            p.IsFolder.Should().Be(isFolder);
        }
    }
}