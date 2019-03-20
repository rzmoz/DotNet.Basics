namespace DotNet.Basics.PowerShell
{
    public class RemoveItemCmdlet : PathCmdlet
    {
        public RemoveItemCmdlet(string path) : base("Remove-Item", path)
        {
        }
    }
}
