using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.ConsoleApp
{
    public class CmdLine
    {
        private const char _quoteChar = '\"'; //quote
        private readonly Dictionary<string, CmdLineParam> _parameters = new Dictionary<string, CmdLineParam>();

        private readonly char[] _paramFlags = { '-', '/' };

        public string this[string key]
        {
            get
            {
                if (!_parameters.ContainsKey(key))
                    throw new KeyNotFoundException(key);
                return _parameters[key]?.Value?.Trim(_quoteChar);
            }
        }

        public bool Exists(string key) => _parameters[key].Exists;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="required"></param>
        /// <param name="allowEmpty"></param>
        /// <param name="readAction">action where you can access the parsed argument - it's run on Parse()</param>
        /// <returns></returns>
        public CmdLine Register(string paramName, Required required, AllowEmpty allowEmpty, Action<CmdLineParam> readAction)
        {
            return Register(new CmdLineParam(paramName) { Required = required, AllowEmptyValue = allowEmpty }, readAction);
        }

        public CmdLine Register(CmdLineParam parameter, Action<CmdLineParam> readAction)
        {
            var key = parameter.Name.ToLower();
            if (_parameters.ContainsKey(key))
                throw new ArgumentException($"Parameter '{key}' is already registered.", key);

            parameter.ReadAction = readAction;
            _parameters.Add(key, parameter);
            return this;
        }

        /// <summary>
        /// Adds a parameter named "debug". Not required and allows empty value. Makes the program wait for input from user so you can attach debugger to console app.
        /// usage [console.exe] -debug
        /// </summary>
        /// <returns></returns>
        public CmdLine RegisterDebug()
        {
            return Register("debug", Required.No, AllowEmpty.Yes, param =>
            {
                if (param.Exists == false)
                    return;
                Console.WriteLine("In debug mode - attach debugger and press a key to continue");
                Console.ReadKey();
                Console.WriteLine("Continuing..");
            });
        }

        public bool Remove(string name)
        {
            var key = name.ToLower();
            if (_parameters.ContainsKey(key))
                return _parameters.Remove(key);
            return false;
        }

        public void ClearParameters()
        {
            _parameters.Clear();
        }

        public bool Parse(params string[] args)
        {
            string error = string.Empty;
            try
            {
                ParseArgs(args);
            }
            catch (ArgumentException ex)
            {
                error = ex.Message;
            }

            if (error != string.Empty)
            {
                Console.WriteLine(Environment.NewLine + error);
                Console.WriteLine(HelpScreen());
                return false;
            }

            //run read actions
            foreach (var cmdLineParam in _parameters)
            {
                cmdLineParam.Value.ReadAction.Invoke(cmdLineParam.Value);
            }

            return true;
        }

        private void ParseArgs(params string[] args)
        {
            int argsPointer = 0;

            while (argsPointer < args.Length)
            {
                var arg = args[argsPointer];

                if (IsParameter(arg))
                {
                    string key = arg.ToLower();
                    key = _paramFlags.Aggregate(key, (current, paramFlag) => current.TrimStart(paramFlag));

                    string value = string.Empty;
                    argsPointer++;

                    bool nextIsValue = true;

                    while (nextIsValue && argsPointer < args.Length)
                    {
                        arg = args[argsPointer];
                        if (IsParameter(arg))
                        {
                            nextIsValue = false;
                        }
                        else
                        {
                            // The next string is a value, read the value and move forward
                            value += "|" + arg;
                            argsPointer++;
                        }
                    }
                    if (!_parameters.ContainsKey(key))
                        throw new ArgumentException("Parameter is not recognized.", key);

                    if (_parameters[key].Exists)
                        throw new ArgumentException("Parameter is specified more than once.", key);

                    _parameters[key].SetValue(value.Trim('|'));
                }
                else
                {
                    argsPointer++;
                }
            }

            CheckRequiredParametersArePresent();
            CheckParametersHaveValues();
        }

        private void CheckParametersHaveValues()
        {
            foreach (var cmdLineParameter in _parameters.Values)
            {
                if (cmdLineParameter.AllowEmptyValue == AllowEmpty.Yes || !cmdLineParameter.Exists)
                    continue;
                if (string.IsNullOrEmpty(cmdLineParameter.Value))
                    throw new ArgumentException("Value is empty.", cmdLineParameter.Name);
            }
        }

        private void CheckRequiredParametersArePresent()
        {
            foreach (string key in _parameters.Keys)
            {
                if (_parameters[key].Required == Required.Yes && !_parameters[key].Exists)
                    throw new ArgumentException("Required parameter is not found.", key);
            }
        }

        private bool IsParameter(string s)
        {
            return s.Length > 0 && _paramFlags.Contains(s[0]);
        }

        public string HelpScreen()
        {
            int len = _parameters.Keys.Select(key => key.Length).Concat(new[] { 0 }).Max();

            string help = "\nParameters:\r\n\r\n";
            foreach (var parameter in _parameters.Values)
            {
                string s = "-" + parameter.Name;
                while (s.Length < len + 3)
                    s += " ";
                if (parameter.Required == Required.Yes)
                    s += "<Required> ";
                s += parameter.Help + Environment.NewLine;
                help += s;
            }
            return help;
        }

    }
}
