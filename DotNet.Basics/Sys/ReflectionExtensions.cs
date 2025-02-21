using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.Sys
{
    public static class ReflectionExtensions
    {
        public static string GetNameWithGenericsExpanded(this Type type)
        {   
            var name = type.Name;
            if (type.IsGenericType)
            {
                name = name.Substring(0, name.IndexOf('`'));
                name += $"<{string.Join(",", type.GetGenericArguments().Select(t => t.GetNameWithGenericsExpanded()).ToArray())}>";
            }
            return name;
        }


        public static bool IsBaseClassOf<T>(this Type baseClass)
        {
            if (baseClass == null) throw new ArgumentNullException(nameof(baseClass));
            return baseClass.IsBaseClassOf(typeof(T));
        }
        public static bool IsBaseClassOf(this Type baseClass, Type? subClass)
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
