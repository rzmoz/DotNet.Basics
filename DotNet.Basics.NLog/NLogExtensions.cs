using System;
using NLog;

namespace DotNet.Basics.NLog
{
    public static class NLogExtensions
    {
        public static Logger NLog<T>(this object @obj)
        {
            return @obj.NLog(typeof(T));
        }
        public static Logger NLog(this object @obj)
        {
            return @obj.NLog(@obj.GetType());
        }
        public static Logger NLog(this object @obj, Type loggerNameType)
        {
            return @obj.NLog(loggerNameType.FullName);
        }
        public static Logger NLog(this object @obj, string loggerName)
        {
            return LogManager.GetLogger(loggerName);
        }
    }
}
