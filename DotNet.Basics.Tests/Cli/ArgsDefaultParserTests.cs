using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Cli
{
    public class ArgsDefaultParserTests(ITestOutputHelper output) : TestWithHelpers(output)
    {
        private readonly IArgsParser _argsParser = new ArgsDefaultParser(false);
        private readonly IArgsParser _argsParserWithVerb = new ArgsDefaultParser(true);

        [Theory]
        [InlineData("lorem", "Lorem", "-hello", "world")]
        public void Parse_Verb_FirstEntryIsVerb(string expectedVerb, params string[] args)
        {
            var dictionary = _argsParserWithVerb.Parse(args);
            dictionary[0].Should().Be(expectedVerb);
            dictionary.ContainsKey("hello").Should().BeTrue();
        }

        [Theory]
        [InlineData("debug", "--debug", "-hello", "my", "world")]//detect flag without flag indicator
        [InlineData("-verbose", "--verbose", "-hello", "my", "world")]//detect flag with simple indicator
        [InlineData("--adO", "--AdO", "-hello", "my", "world")]//detect flag with full indicator
        [InlineData("/debug", "/debug", "-hello", "my", "world")]//detect flag with mixed indicators
        public void Parse_ReservedFlags_NotInDictionary(string flag, params string[] args)
        {
            var dictionary = _argsParser.Parse(args);
            dictionary.ContainsKey(flag).Should().Be(false);
        }

        [Theory]
        [InlineData(true, true, true, "--debug", "-aDO", "/verbose", "--world")]
        [InlineData(false, true, true, "--FFFdebug", "-aDO", "/verbose", "--world")]
        [InlineData(true, false, true, "--debug", "-SSSSaDO", "//verbose")]
        [InlineData(true, true, false, "--debug", "-aDO", "--world")]
        public void Parse_ReservedFlags_AreParsed(bool debug, bool ADO, bool verbose, params string[] args)
        {
            var dictionary = _argsParser.Parse(args);
            dictionary.Verbose.Should().Be(verbose);
            dictionary.ADO.Should().Be(ADO);
            dictionary.Debug.Should().Be(debug);
        }

        [Fact]
        public void Parse_KeyValueStyle_ArgsAreParsed()
        {
            var key = "-/-//-//---/-/-/myKey";//annoyingly mixed flag indicator => no error
            var value = $"1|2|3";
            string[] args = [key, value, "--anotherFlag"];
            var dictionary = _argsParser.Parse(args);
            dictionary[key].Should().Be(value);
        }

    }
}
