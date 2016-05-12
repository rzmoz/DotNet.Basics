using System;

namespace DotNet.Basics.Sys
{
    public static class PowerShellCmdletsExtensions
    {
        public static object[] Run(this PowerShellCmdlet cmdlet)
        {
            if (cmdlet == null) throw new ArgumentNullException(nameof(cmdlet));
            return PowerShellConsole.RunCommand(cmdlet);
        }
    }
}
