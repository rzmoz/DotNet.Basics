using DotNet.Basics.Serilog.Looging;
using DotNet.Basics.Win;

namespace DotNet.Basics.Cli
{
    public static class ILoogExtensions
    {
        public static CmdPromptLogger WithPromptLogger(this ILoog log, LoogLevel debug = LoogLevel.Verbose, LoogLevel info = LoogLevel.Verbose, LoogLevel error = LoogLevel.Error)
        {
            var cmdLogger = new CmdPromptLogger();
            cmdLogger.DebugLogged += d => log.Write(debug, d);
            cmdLogger.InfoLogged += i => log.Write(info, i);
            cmdLogger.ErrorLogged += e => log.Write(error, e);
            return cmdLogger;
        }
    }
}
