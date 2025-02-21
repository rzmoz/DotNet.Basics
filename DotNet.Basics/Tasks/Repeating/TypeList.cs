using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks.Repeating
{
    public class TypeList : List<Type>
    {
        public TypeList() { }

        public TypeList(IEnumerable<Type> collection) : base(collection) { }

        public TypeList(int capacity) : base(capacity) { }

        public void Add<T>()
        {
            Add(typeof(T));
        }

        public void AddRange(params Type[] types)
        {
            base.AddRange(types);
        }
    }
}
