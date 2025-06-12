using DotNet.Basics.Diagnostics;
using DotNet.Basics.Tasks;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public class CliHost(CliHostOptions options)
    {
        private const string _newlinePattern = @"\r\n|\r|\n";
        private static readonly Regex _newlineRegex = new(_newlinePattern, RegexOptions.Compiled);
        public CliHostOptions Options { get; } = options;
        public ArgsDictionary Args => Options.Args;

        public async Task<int> RunPipelineAsync<T>(PipelineArgsFactory? pipelineArgsFactory = null, ILogger? logger = null) where T : ManagedTask
        {
            try
            {
                logger ??= Options.GetService<ILogger>();
                var argsFactory = pipelineArgsFactory ?? new PipelineArgsFactory();
                var args = argsFactory.Create(typeof(T), Args, logger);
                return await RunPipelineAsync<T>(args, logger);
            }
            catch (MissingArgumentException e)
            {
                var log = Options.GetService<ILogger>();
                log.Error(
                    $"\r\nMissing argument(s): {e.ArgsType.FullName?.Highlight() ?? e.ArgsType.Name.Highlight()}");
                foreach (var missingArg in e.MissingArgs)
                    log.Error(" - [{argType}] {argName} is not set!", missingArg.ArgType, missingArg.ArgName.Highlight());
                return 400;
            }
            catch (UnknownArgumentsException e)
            {
                var log = Options.GetService<ILogger>();
                log.Error($"\r\nUnknown argument(s): {e.ArgNames.JoinString(", ").Highlight()}");
                return 400;
            }
            catch (Exception e)
            {
                var log = Options.GetService<ILogger>();
                log.Error($"\r\n{e.GetType().FullName}: {e.Message.Highlight()} ");
                log.Trace(string.Join(Environment.NewLine, _newlineRegex.Split(e.ToString()).Skip(1)));
                return 400;
            }
        }

        public async Task<int> RunPipelineAsync<T>(object args, ILogger? logger = null) where T : ManagedTask
        {
            var managedTask = Options.GetService<T>();
            logger ??= Options.GetService<ILogger>();
            managedTask.Started += taskName =>
            {
                if (!taskName.EndsWith("Pipeline"))
                    logger.Debug($"{taskName.Highlight()} started");
            };
            managedTask.Ended += (taskName, e) =>
            {
                if (!taskName.EndsWith("Pipeline"))
                    logger.Debug($"{taskName.Highlight()} ended");
            };

            return await RunPipelineAsync(managedTask, args);
        }

        public async Task<int> RunPipelineAsync(ManagedTask managedTask, object args)
        {
            return await RunAsync(managedTask.Name, () => managedTask.RunAsync(args));
        }
        public async Task<int> RunAsync(string operationName, Func<Task<int>> loogContext)
        {
            var log = Options.GetService<ILogger>();
            var longRunningOperations = Options.GetService<LongRunningOperations>();
            var exitCode = Options.FatalExitCode;

            try
            {
                exitCode = await longRunningOperations.StartAsync(operationName, loogContext.Invoke);
            }
            catch (Exception e)
            {
                log.Error($"\r\n{e.GetType().FullName}: {e.Message.Highlight()} ");
                log.Trace(string.Join(Environment.NewLine, _newlineRegex.Split(e.ToString()).Skip(1)));

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
                log.Debug($"Exit code: {exitCode.ToString().Highlight()}");
                Console.ResetColor();
            }
            return exitCode;
        }
    }
}
