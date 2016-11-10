using System;
using Autofac;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class Pipeline : Pipeline<EventArgs>
    {
        public Pipeline()
        { }

        public Pipeline(string name) : base(name)
        { }

        public Pipeline(IContainer container) : base(container)
        { }

        public Pipeline(Invoke invoke) : base(invoke)
        { }

        public Pipeline(string name, Invoke invoke) : base(name, invoke)
        { }

        public Pipeline(IContainer container, Invoke invoke) : base(container, invoke)
        { }

        public Pipeline(string name, IContainer container, Invoke invoke) : base(name, container, invoke)
        { }
    }
    public class Pipeline<T> : ManagedBlock<T> where T : class, new()
    {
        public Pipeline()
        { }

        public Pipeline(string name) : base(name)
        { }

        public Pipeline(IContainer container) : base(container)
        { }

        public Pipeline(Invoke invoke) : base(invoke)
        { }

        public Pipeline(string name, Invoke invoke) : base(name, invoke)
        { }

        public Pipeline(IContainer container, Invoke invoke) : base(container, invoke)
        { }

        public Pipeline(string name, IContainer container, Invoke invoke) : base(name, container, invoke)
        { }
    }
}
