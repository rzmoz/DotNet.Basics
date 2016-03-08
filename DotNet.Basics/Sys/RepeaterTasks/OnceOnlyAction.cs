using System;

namespace CSharp.Basics.Sys.RepeaterTasks
{
    public class OnceOnlyAction
    {
        public OnceOnlyAction(Action doOnceAction)
        {
            DoOnceAction = doOnceAction;
            ActionWrapper = CancelAction;
        }

        //we need to wrap it in an actual method since the action might otherwise be referenced directly when used externally
        public void Invoke()
        {
            ActionWrapper.Invoke();
        }

        private Action ActionWrapper { get; set; }

        private Action DoOnceAction { get; set; }

        private void CancelAction()
        {
            try
            {
                DoOnceAction.Invoke();
            }
            catch (Exception)
            {
                // ignored
            }
            ActionWrapper = NullAction;
        }

        private void NullAction()
        {
            //do nothing
        }
    }
}
