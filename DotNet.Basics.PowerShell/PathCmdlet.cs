namespace DotNet.Basics.PowerShell
{
    public abstract class PathCmdlet : PowerShellCmdlet
    {
        protected PathCmdlet(string name, string path) : base(name)
        {
            WithParam(nameof(path), new[] { path });
        }
    }
}
