using DotNet.Basics.Serilog.Looging;
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

            ILoog outerLoog = new Loog();
            var innerLog = outerLoog.InContext(context1).InContext(context2);
            var messageReceived = string.Empty;

            outerLoog.MessageLogged += (lvl, msg, e) => { messageReceived = msg; };

            //act
            innerLog.Info(message);

            messageReceived.Should().Be($"{context1} / {context2} / {message}");
        }
    }
}
