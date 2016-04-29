using System.Collections.Generic;

namespace DotNet.Basics.Sys
{
    internal static class PowerShellExtensions
    {
        public static string WithErrorAction(this string script, bool enable = true, string errorAction = "SilentlyContinue")
        {
            return script.WithScriptParam(enable, $"ErrorAction {errorAction}");
        }
        public static string WithRecurse(this string script, bool enable = true)
        {
            return script.WithScriptParam(enable, "Recurse");
        }
        public static string WithForce(this string script, bool enable = true)
        {
            return script.WithScriptParam(enable, "Force");
        }
        public static string WithScriptParam(this string script, bool enable, string param)
        {
            if (enable)
                return $"{script} -{param}";
            return script;
        }
        public static string ToPowerShellParameterString(this IEnumerable<string> array)
        {
            const char paramDivider = ',';
            var paramString = "(";
            foreach (var str in array)
            {
                paramString += $"\"{str}\"{paramDivider }";
            }
            paramString = paramString.TrimEnd(paramDivider);
            paramString += ")";
            return paramString;
        }
    }
}
