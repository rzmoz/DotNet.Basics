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
    public class TempWorkDirTests
    {
        [Test]
        public void Use_Dir_DirExists()
        {
            DirectoryInfo dir = null;

            using (var temp = new TempDir())
            {
                dir = temp.Dir;
                dir.Exists().Should().BeTrue();
            }

            dir.Exists().Should().BeFalse();
        }
    }
}
