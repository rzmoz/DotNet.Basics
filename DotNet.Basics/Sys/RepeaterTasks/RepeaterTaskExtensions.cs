using System;

namespace CSharp.Basics.Sys.RepeaterTasks
{
    public static class RepeaterTaskExtensions
    {
        public static T WithNoRetryDelay<T>(this T repeaterTask) where T : RepeaterTask
        {
            repeaterTask.RetryDelay = 1.MilliSeconds();
            return repeaterTask;
        }
        public static T WithRetryDelay<T>(this T repeaterTask, TimeSpan retryDelay) where T : RepeaterTask
        {
            repeaterTask.RetryDelay = retryDelay;
            return repeaterTask;
        }

        public static T IgnoreExceptionsOfType<T>(this T repeaterTask, Type exceptionTypeToIgnore) where T : RepeaterTask
        {
            repeaterTask.InteralIgnoreExceptionsOfType(exceptionTypeToIgnore);
            return repeaterTask;
        }

        public static T WithPing<T>(this T repeaterTask, Action pingAction) where T : RepeaterTask
        {
            repeaterTask.Ping = pingAction;
            return repeaterTask;
        }

        public static T WithTimeout<T>(this T repeaterTask, TimeSpan timeout) where T : RepeaterTask
        {
            repeaterTask.TimeoutLoopBreakPredicate = new TimeoutLoopBreakPredicate(timeout);
            return repeaterTask;
        }
        public static T WithMaxTries<T>(this T repeaterTask, uint maxTries) where T : RepeaterTask
        {
            repeaterTask.CountLoopBreakPredicate = new CountLoopBreakPredicate(maxTries);
            return repeaterTask;
        }

        public static T Until<T>(this T repeaterTask, Func<bool> untilPredicate) where T : RepeaterTask
        {
            repeaterTask.UntilPredicate = (e) => untilPredicate.Invoke();
            return repeaterTask;
        }

        public static T UntilNoExceptions<T>(this T repeaterTask) where T : RepeaterTask
        {
            repeaterTask.UntilPredicate = (e) => e == null;
            return repeaterTask;
        }
    }
}
