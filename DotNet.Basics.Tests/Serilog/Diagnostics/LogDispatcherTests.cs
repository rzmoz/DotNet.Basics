using DotNet.Basics.Serilog.Diagnostics;
using FluentAssertions;
using Xunit;


namespace DotNet.Basics.Tests.Serilog.Diagnostics
{
    public class LogDispatcherTests
    {
        [Fact]
        public void Context_PushContext_ContextIsAddedToMessages()
        {
            var context1 = "context1";
            var context2 = "context2";
            var message = "my Message";

            ILogger outerLog = new Logger();
            var innerLog = outerLog.InContext(context1).InContext(context2);
            var messageReceived = string.Empty;

            outerLog.MessageLogged += (lvl, msg, e) => { messageReceived = msg; };

            //act
            innerLog.Info(message);

            messageReceived.Should().Be($"{context1} / {context2} / {message}");
        }
    }
}
