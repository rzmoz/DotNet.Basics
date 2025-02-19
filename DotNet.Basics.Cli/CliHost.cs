using DotNet.Basics.Serilog.Looging;
using DotNet.Basics.Tasks;
using Serilog;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli
{
    public class CliHost(CliHostOptions options) : IAsyncDisposable
    {
        private const string _newlinePattern = @"\r\n|\r|\n";
        private static readonly Regex _newlineRegex = new(_newlinePattern, RegexOptions.Compiled);
        public CliHostOptions Options { get; } = options;
        public ArgsDictionary Args => Options.Args;

        public async Task<int> RunPipelineAsync<T>() where T : ManagedTask
        {
            var argsFactory = new PipelineArgsFactory();
            var args = argsFactory.Create(typeof(T), Args);
            return await RunPipelineAsync<T>(args);
        }

        public async Task<int> RunPipelineAsync<T>(object args) where T : ManagedTask
        {
            var managedTask = Options.GetService<T>();
            return await RunPipelineAsync(managedTask, args);
        }
        public async Task<int> RunPipelineAsync(ManagedTask managedTask, object args)
        {
            return await RunAsync(managedTask.Name, () => managedTask.RunAsync(args));
        }
        public async Task<int> RunAsync(string operationName, Func<Task<int>> loogContext)
        {
            var log = Options.GetService<ILoog>()!;
            var longRunningOperations = Options.GetService<LongRunningOperations>();
            var exitCode = Options.FatalExitCode;

            try
            {
                exitCode = await longRunningOperations.StartAsync(operationName, () => loogContext.Invoke());
            }
            catch (CliArgNotFoundException e)
            {
                log.Info(" ");
                log.Fatal($"Missing argument: {e.Message}");
                exitCode = 400;
            }
            catch (Exception e)
            {
                log.Info(" ");
                log.Fatal($"{e.GetType().FullName}: {e.Message.Highlight()} ");
                log.Error(string.Join(Environment.NewLine, _newlineRegex.Split(e.ToString()).Skip(1)));

                var exitCodeProperty = e.GetType().GetProperty("ExitCode");
                if (exitCodeProperty != null)
                {
                    var rawValue = exitCodeProperty.GetValue(e);
                    if (rawValue != null)
                    {
                        exitCode = int.Parse(rawValue.ToString()!, NumberStyles.Integer);
                    }
                }
            }
            finally
            {
                log.Write(exitCode == 0 ? LoogLevel.Success : LoogLevel.Error, $"Global exit code: {exitCode.ToString().Highlight()}");
                Console.ResetColor();
            }
            return exitCode;
        }

        public async ValueTask DisposeAsync()
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
