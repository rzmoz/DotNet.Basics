using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.Reflection
{
    public static class ReflectionExtensions
    {
        public static bool IsBaseClassOf<T>(this Type baseClass)
        {
            if (baseClass == null) throw new ArgumentNullException(nameof(baseClass));
            return baseClass.IsBaseClassOf(typeof(T));
        }
        public static bool IsBaseClassOf(this Type baseClass, Type subClass)
        {
            if (baseClass == null) throw new ArgumentNullException(nameof(baseClass));
            while (subClass != null && subClass != typeof(object))
            {
                var cur = subClass.IsGenericType ? subClass.GetGenericTypeDefinition() : subClass;
                if (baseClass == cur)
                    return true;
                subClass = subClass.BaseType;
            }
            return false;
        }

        public static IEnumerable<Type> GetTypesOf(this Assembly assembly, Type typeOf)
        {
            return assembly.GetTypes().Where(t => typeOf.IsBaseClassOf(t) && t.IsAbstract == false);
        }
    }
}
