using NLog;

namespace DotNet.Basics.NLog
{
    public static class NLogExtensions
    {
        public static Logger NLog<T>(this object @obj)
        {
            return @obj.NLog(typeof(T).FullName);
        }
        public static Logger NLog(this object @obj)
        {
            return @obj.NLog(@obj.GetType().FullName);
        }
        public static Logger NLog(this object @obj, string loggerName)
        {
            return LogManager.GetLogger(loggerName);
        }
    }
}
