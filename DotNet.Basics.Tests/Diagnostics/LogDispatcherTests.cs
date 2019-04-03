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

            var log = new LogDispatcher();
            var messageReceived = string.Empty;
            log.PushContext(context1)
               .PushContext(context2);
            log.MessageLogged += (lvl, msg, e) => { messageReceived = msg; };

            //act
            log.Information(message);

            messageReceived.Should().Be($"{context1} / {context2} / {message}");
        }
    }
}
