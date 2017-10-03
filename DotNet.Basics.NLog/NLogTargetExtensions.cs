using NLog.Targets;
using NLog.Targets.Wrappers;

namespace DotNet.Basics.NLog
{
    public static class NLogTargetExtensions
    {
        /// <summary>
        /// https://github.com/nlog/NLog/wiki/BufferingWrapper-target
        /// </summary>
        /// <param name="wrappedTarget"></param>
        /// <param name="bufferSize"></param>
        /// <param name="flushTimeout"></param>
        /// <returns></returns>
        public static BufferingTargetWrapper AsBuffered(this Target wrappedTarget, int bufferSize = 100,
            int flushTimeout = 1000)
        {
            return new BufferingTargetWrapper(wrappedTarget, bufferSize, flushTimeout);
        }

        /// <summary>
        /// https://github.com/nlog/NLog/wiki/AsyncWrapper-target
        /// </summary>
        /// <param name="wrappedTarget"></param>
        /// <param name="queueLimit"></param>
        /// <param name="overflowAction"></param>
        /// <returns></returns>
        public static AsyncTargetWrapper AsAsync(this Target wrappedTarget, int queueLimit = 10000, AsyncTargetWrapperOverflowAction overflowAction = AsyncTargetWrapperOverflowAction.Grow)
        {
            return new AsyncTargetWrapper(wrappedTarget, queueLimit, overflowAction);
        }
    }
}
