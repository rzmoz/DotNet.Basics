using DotNet.Basics.Diagnostics;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Diagnostics
{
    public class LogDispatcherTests
    {
        [Fact]
        public void Context_PushContext_ContextIsAddedToMessages()
        {
            var context1 = "context1";
            var context2 = "context2";
            var message = "my Message";

            var outerLog = new LogDispatcher();
            var innerLog = new LogDispatcher();
            var messageReceived = string.Empty;
            outerLog.PushContext(context1);
            innerLog.PushContext(context2);
            innerLog.MessageLogged += outerLog.Write;
            outerLog.MessageLogged += (lvl, msg, e) => { messageReceived = msg; };

            //act
            innerLog.Information(message);

            messageReceived.Should().Be($"{context1} / {context2} / {message}");
        }
    }
}
