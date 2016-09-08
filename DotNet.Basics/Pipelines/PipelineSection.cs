using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineSection<T> where T : EventArgs
    {
        public delegate void SectionStartedEventHandler(SectionStartedEventArgs<T> args);
        public delegate void SectionEndedEventHandler(SectionEndedEventArgs<T> args);

        public event SectionStartedEventHandler SectionStarted;
        public event SectionEndedEventHandler SectionEnded;

        protected PipelineSection(string name)
        {
            Name = name ?? GetType().Name;
        }

        public string Name { get; protected set; }
        public abstract SectionType SectionType { get; }

        public virtual void Init()
        { }

        public async Task<T> RunAsync(T args, CancellationToken ct)
        {
            Exception exceptionEncountered = null;
            try
            {
                Init();
                FireSectionStarted(new SectionStartedEventArgs<T>(Name, SectionType, args));
                await InnerRunAsync(args, ct).ConfigureAwait(false);
                return args;
            }
            catch (Exception e)
            {
                exceptionEncountered = e;
                throw;
            }
            finally
            {
                FireSectionEnded(new SectionEndedEventArgs<T>(Name, SectionType, args, ct.IsCancellationRequested, exceptionEncountered));
            }
        }

        protected abstract Task InnerRunAsync(T args, CancellationToken ct);

        protected void FireSectionStarted(SectionStartedEventArgs<T> args)
        {
            SectionStarted?.Invoke(args);
        }
        protected void FireSectionEnded(SectionEndedEventArgs<T> args)
        {
            SectionEnded?.Invoke(args);
        }
    }
}
