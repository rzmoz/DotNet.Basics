using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class TempWorkFileTests
    {
        [Test]
        public void Use_File_FileExists()
        {
            FileInfo file = null;

            var tempWorkFile = new TempWorkFile();
            tempWorkFile.Use(f =>
            {
                file = f;
                file.Exists().Should().BeTrue();
            });
            file.Exists().Should().BeFalse();
        }
    }
}
