using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask<T> where T : EventArgs, new()
    {
        private readonly Func<T, CancellationToken, Task> _task;
        private string _name;

        public delegate void TaskStartedEventHandler(TaskStartedEventArgs args);
        public delegate void TaskEndedEventHandler(TaskEndedEventArgs args);

        public ManagedTask(Func<Task> task) : this((args, ct) => task())
        {
        }
        public ManagedTask(Action task) : this((args, ct) => task())
        {

        }

        public ManagedTask(Action<T, CancellationToken> task)
            : this((args, ct) =>
             {
                 task(args, ct);
                 return Task.CompletedTask;
             })
        {
        }

        public ManagedTask(Func<T, CancellationToken, Task> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _task = task;
            Name = GetType().FullName;
            Properties = new StringDictionary(DictionaryKeyMode.IgnoreKeyCase);
        }

        public event TaskStartedEventHandler TaskStarted;
        public event TaskEndedEventHandler TaskEnded;

        public string Name
        {
            get { return _name; }
            set { _name = value ?? GetType().FullName; }
        }

        public StringDictionary Properties { get; }

        public virtual void Init()
        {
        }

        public Task<T> RunAsync()
        {
            return RunAsync(new T(), CancellationToken.None);
        }

        public Task<T> RunAsync(T args)
        {
            return RunAsync(args, CancellationToken.None);
        }

        public Task<T> RunAsync(CancellationToken ct)
        {
            return RunAsync(new T(), ct);
        }

        public async Task<T> RunAsync(T args, CancellationToken ct)
        {
            Exception exceptionEncountered = null;
            if (args == null)
                args = new T();
            try
            {
                Init();
                TaskStarted?.Invoke(new TaskStartedEventArgs(Name, Properties));
                await _task(args, ct).ConfigureAwait(false);
                return args;
            }
            catch (Exception e)
            {
                exceptionEncountered = e;
                throw;
            }
            finally
            {
                TaskEnded?.Invoke(new TaskEndedEventArgs(Name, Properties, ct.IsCancellationRequested, exceptionEncountered));
            }
        }
    }
}