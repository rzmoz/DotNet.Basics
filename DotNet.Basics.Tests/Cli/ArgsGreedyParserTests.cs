using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Cli
{
    public class ArgsGreedyParserTests(ITestOutputHelper output) : TestWithHelpers(output)
    {
        private readonly IArgsParser _argsParser = new ArgsGreedyParser();

        [Theory]
        [InlineData("debug", true, "--debug", "-hello", "my", "world")]//detect flag without flag indicator
        [InlineData("-debug", true, "--debug", "-hello", "my", "world")]//detect flag with simple indicator
        [InlineData("--debug", true, "--debug", "-hello", "my", "world")]//detect flag with full indicator
        [InlineData("/debug", true, "/debug", "-hello", "my", "world")]//detect flag with mixed indicators
        public void Parse_DebugFlag_FlagIsDetected(string flag, bool flagIsPresent, params string[] args)
        {
            var dictionary = _argsParser.Parse(args);

            dictionary.ContainsKey(flag).Should().Be(flagIsPresent);
        }

        [Fact]
        public void Parse_MultipleValues_MultiValuesAreSupport()
        {
            var key = "--myKey";
            string[] args = [key, "1", "2", "3", "--anotherFlag"];
            var dictionary = _argsParser.Parse(args);
            dictionary.Get(key).Count.Should().Be(3);
        }

        [Fact]
        public void Parse_KeyValueStyle_ArgsAreParsed()
        {
            var key = "---------myKey";//annoyingly long flag indicator => no error
            string[] args = [$"{key}=1|2|3", "--anotherFlag"];
            var dictionary = _argsParser.Parse(args);
            dictionary.Get(key).Count.Should().Be(3);
        }

        [Fact]
        public void Parse_ConfusedFlagStyles_ArgsAreParsed()
        {
            var key = "-/-//-//---/-/-/myKey";//annoyingly mixed flag indicator => no error
            string[] args = [$"{key}=1|2|3", "--anotherFlag"];
            var dictionary = _argsParser.Parse(args);
            dictionary.Get(key).Count.Should().Be(3);
        }
    }
}
