using Serilog;
using System.Threading.Tasks;
using System;
using System.Reflection;
using DotNet.Basics.Serilog;

namespace DotNet.Basics.Cli
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">set T to Program (main entry class)</typeparam>
    public static class HostExtensions
    {
        private static readonly string _entryNamespace = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;

        public static T BuildManaged<T>(this T builder, Action<LoggerConfiguration, T> configureBuilder)
        {
            try
            {
                SerilogInit.SetGlobalLogger();
                LogApplicationEvent("Initializing...");
                var userLogConfig = SerilogInit.GetDefaultConfiguration();
                configureBuilder(userLogConfig, builder);
                SerilogInit.SetGlobalLogger(userLogConfig);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.ToString());
                LogApplicationEvent("Configure Builder failed and aborting...");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return builder;
        }
        public static async Task<int> RunManagedAsync<T>(this T host, Func<T, Task> runManagedAsync)
        {
            try
            {
                LogApplicationEvent("Run Host starting...");
                await runManagedAsync(host).ConfigureAwait(false);
                return 0;
            }
            catch (Exception ex)
            {

                Log.Logger.Error(ex, ex.ToString());
                LogApplicationEvent("Run Host failed and aborting...");
            }
            finally
            {
                await Log.CloseAndFlushAsync().ConfigureAwait(false);
            }
            return -500;//fatal error
        }
        private static void LogApplicationEvent(string @event)
        {
            Log.Information($">>>>>>>>>> {{serviceName}} {@event} <<<<<<<<<<", _entryNamespace);
        }
    }

}
