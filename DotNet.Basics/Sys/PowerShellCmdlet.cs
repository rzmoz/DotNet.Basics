using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Sys
{
    public class PowerShellCmdlet
    {
        private readonly IList<KeyValuePair<string, object>> _parameters;

        public PowerShellCmdlet(string name, params KeyValuePair<string, object>[] parameters)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Name = name;
            _parameters = new List<KeyValuePair<string, object>>(); ;
        }

        public string Name { get; }
        public KeyValuePair<string, object>[] Parameters => _parameters.ToArray();

        public PowerShellCmdlet AddParameter(string name, params string[] values)
        {
            _parameters.Add(new KeyValuePair<string, object>(name, ToPowerShellParameterString(values)));
            return this;
        }

        public PowerShellCmdlet WithErrorAction(string action)
        {
            return AddParameter("ErrorAction", action);
        }

        public PowerShellCmdlet WithForce(bool force)
        {
            if (force)
                return AddParameter("Force");
            return this;
        }

        public PowerShellCmdlet WithRecurse(bool recurse)
        {
            if (recurse)
                return AddParameter("Recurse");
            return this;
        }

        public string ToScript()
        {
            var script = Name;
            foreach (var param in Parameters)
            {
                script += $" -{param.Key}";

                var value = param.Value?.ToString();

                if (string.IsNullOrEmpty(value) == false)
                    script += $" {value}";
            }

            return script.TrimEnd();
        }


        private string ToPowerShellParameterString(string[] array)
        {
            //no values
            if (array.Length == 0)
                return null;

            //single value
            if (array.Length == 1)
                return $"\"{array[0]}\"";

            //multiple values
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
