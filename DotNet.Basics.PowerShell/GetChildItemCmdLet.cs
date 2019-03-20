namespace DotNet.Basics.PowerShell
{
    public class GetChildItemCmdlet : PathCmdlet
    {
        public GetChildItemCmdlet(string path) : base("Get-ChildItem", path)
        {
        }

        public GetChildItemCmdlet WithFilter(string filter)
        {
            WithParam(nameof(filter), filter);
            return this;
        }
    }
}
