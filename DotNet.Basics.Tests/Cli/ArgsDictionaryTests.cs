using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Cli
{
    public class ArgsDictionaryTests(ITestOutputHelper output) : TestWithHelpers(output)
    {
        private readonly IArgsParser _argsParserWithVerb = new ArgsDefaultParser(true);

        [Theory]
        [InlineData("lorem", "Lorem", "-hello", "world")]
        public void Parse_Verb_FirstEntryIsVerb(string expectedVerb, params string[] args)
        {
            var dictionary = _argsParserWithVerb.Parse(args);
            dictionary.FirstEntryIsVerb.Should().BeTrue();
            dictionary[0].Should().Be(expectedVerb);
            dictionary.ContainsKey("hello").Should().BeTrue();
        }
    }
}
