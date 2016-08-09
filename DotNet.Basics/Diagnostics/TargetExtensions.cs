using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace DotNet.Basics.Diagnostics
{
    public static class TargetExtensions
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
