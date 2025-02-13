using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.IO.Testa
{
    public class TestFile1(DirPath testRootPath) : FilePath(@$"{testRootPath}/IO/Testa", "TextFile1.txt");
}
