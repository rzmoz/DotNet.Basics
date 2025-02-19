using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Cli
{
    public class PipelinesFactoryTests(ITestOutputHelper output) : TestWithHelpers(output)
    {
        private readonly PipelineArgsFactory _argsFactory = new();

        [Fact]
        public void GetParser_SimpleTypes_TypeAreSupported()
        {
            Assert<string>("My string");
            Assert<bool>(true);
            Assert<bool>(false);
            Assert<byte>(byte.MaxValue);
            Assert<sbyte>(sbyte.MaxValue);
            Assert<char>('*');
            Assert<decimal>(decimal.One);
            Assert<double>(double.MaxValue);
            Assert<float>(float.MaxValue);
            Assert<int>(int.MaxValue);
            Assert<uint>(uint.MaxValue);
            Assert<nint>(nint.MaxValue);
            Assert<nuint>(nuint.MaxValue);
            Assert<long>(long.MaxValue);
            Assert<ulong>(ulong.MaxValue);
            Assert<short>(short.MaxValue);
            Assert<ushort>(ushort.MaxValue);
        }

        private void Assert<T>(T arg)
        {
            var result = _argsFactory.GetParser(typeof(T)).Invoke(arg.ToString());
            result.Should().Be((T)arg);
        }
    }
}
