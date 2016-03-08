using System;

namespace DotNet.Basics.Tasks.Repeating
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
            finally
            {
                ActionWrapper = VoidAction;//set action to void to ensure it's only ever run once    
            }
        }

        private void VoidAction()
        {
            //do nothing
        }
    }
}
