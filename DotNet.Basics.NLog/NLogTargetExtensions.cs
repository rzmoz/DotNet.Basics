using NLog.Targets;
using NLog.Targets.Wrappers;

namespace DotNet.Basics.NLog
{
    public static class NLogTargetExtensions
    {
        public static BufferingTargetWrapper AsBuffered(this Target wrappedTarget, int bufferSize = 100,
            int flushTimeout = 1000)
        {
            return new BufferingTargetWrapper(wrappedTarget, bufferSize, flushTimeout);
        }

        public static AsyncTargetWrapper AsAsync(this Target wrappedTarget, int queueLimit = 10000, AsyncTargetWrapperOverflowAction overflowAction = AsyncTargetWrapperOverflowAction.Grow)
        {
            return new AsyncTargetWrapper(wrappedTarget, queueLimit, overflowAction);
        }
    }
}
