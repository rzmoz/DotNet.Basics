using System;

namespace DotNet.Basics.Sys
{
    public static class TypeExtensions
    {
        public static bool Is<T>(this object @object)
        {
            if (@object == null)
                return false;

            return @object.GetType().Is<T>();
        }

        public static bool Is<T>(this Type @type)
        {
            if (type == null)
                return false;
            return @type.FullName.Equals(typeof(T).FullName);
        }
    }
}
