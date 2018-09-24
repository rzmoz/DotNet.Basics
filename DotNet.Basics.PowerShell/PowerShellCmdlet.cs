using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace DotNet.Basics.PowerShell
{
    public class PowerShellCmdlet
    {
        private readonly IList<KeyValuePair<string, object>> _parameters;

        public PowerShellCmdlet(string name, params KeyValuePair<string, object>[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _parameters = new List<KeyValuePair<string, object>>(); ;
        }

        public string Name { get; }
        public KeyValuePair<string, object>[] Parameters => _parameters.ToArray();

        public PowerShellCmdlet AddParameter(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            foreach (var parameter in parameters)
                _parameters.Add(parameter);
            return this;
        }

        public PowerShellCmdlet AddParameter(string name, object value = null)
        {
            _parameters.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public PowerShellCmdlet WithErrorAction(ActionPreference errorAction)
        {
            return AddParameter(nameof(errorAction), errorAction);
        }

        public PowerShellCmdlet WithForce(bool force = true)
        {
            return force ? AddParameter(nameof(force)) : this;
        }

        public PowerShellCmdlet WithRecurse(bool recurse = true)
        {
            return recurse ? AddParameter(nameof(recurse)) : this;
        }

        public PowerShellCmdlet WithVerbose(bool verbose = true)
        {
            return verbose ? AddParameter(nameof(verbose)) : this;
        }

        public override string ToString()
        {
            var script = Name;
            foreach (var param in Parameters)
            {
                script += $" -{param.Key}";

                var value = param.Value?.ToString();
                if (value != null)
                    value = $"\"{value}\"";
                if (param.Value is string[] strings)
                    value = ToPowerShellParameterString(strings);

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
