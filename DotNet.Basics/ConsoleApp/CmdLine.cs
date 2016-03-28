using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Collections;

namespace DotNet.Basics.ConsoleApp
{
    public class CmdLine : IReadOnlyCollection<CmdLineParam>
    {
        private const string _debugParamName = "debug";

        private readonly StringKeyDictionary<CmdLineParam> _parameters = new StringKeyDictionary<CmdLineParam>(KeyMode.CaseInsensitive, KeyNotFoundMode.ReturnDefault);

        private readonly char[] _paramFlags = { '-', '/' };

        public CmdLine(ValueMode valueMode = ValueMode.Raw)
        {
            ValueMode = valueMode;
            RegisterDebug();
        }

        public CmdLineParam this[string key] => _parameters[key];

        /// <summary>
        /// Registers all public string properties with same settings. Change restrictions using GetParam(string paramName)
        /// </summary>
        public T Register<T>(Required required = Required.Yes, AllowEmpty allowEmpty = AllowEmpty.No) where T : new()
        {
            var publicPropertiesWithSetter = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public).Where(prop => prop.GetSetMethod() != null).Where(prop => prop.PropertyType == typeof(string));

            var t = new T();
            foreach (var propertyInfo in publicPropertiesWithSetter)
            {
                Register(propertyInfo.Name, required, allowEmpty, p =>
                 {
                     propertyInfo.SetValue(t, p.Value);
                 });
            }
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="required"></param>
        /// <param name="allowEmpty"></param>
        /// <param name="readAction">action where you can access the parsed argument - it's run on Parse()</param>
        /// <param name="help"></param>
        /// <returns></returns>
        public CmdLine Register(string paramName, Required required, AllowEmpty allowEmpty, Action<CmdLineParam> readAction, string help = null)
        {
            return Register(new CmdLineParam(paramName) { Required = required, AllowEmptyValue = allowEmpty, Help = help ?? string.Empty }, readAction);
        }

        public CmdLine Register(CmdLineParam parameter, Action<CmdLineParam> readAction)
        {
            if (_parameters.ContainsKey(parameter.Name))
                throw new ArgumentException($"Parameter '{parameter.Name}' is already registered.", parameter.Name);

            parameter.ReadAction = readAction;
            _parameters.Add(parameter.Name, parameter);
            return this;
        }

        public CmdLine Remove(string name)
        {
            if (name.Equals(_debugParamName, StringComparison.CurrentCultureIgnoreCase))
                return this;//we don't remove the debug param
            _parameters.Remove(name);
            return this;
        }

        public void ClearParameters()
        {
            _parameters.Clear();
            RegisterDebug();
        }
        public bool Parse(string[] args, WriteErrorMessagesToConsole writeErrorMessagesToConsole = WriteErrorMessagesToConsole.True)
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
                if (writeErrorMessagesToConsole == WriteErrorMessagesToConsole.True)
                {
                    Console.WriteLine(Environment.NewLine + error);
                    Console.WriteLine(HelpScreen());
                }
                return false;
            }

            //run read actions
            foreach (var cmdLineParam in _parameters)
            {
                cmdLineParam.Value.ReadAction.Invoke(cmdLineParam.Value);
            }

            return true;
        }


        public string HelpScreen()
        {
            int len = _parameters.Keys.Select(key => key.Length).Concat(new[] { 0 }).Max();

            string help = "\nParameters:\r\n\r\n";
            foreach (var parameter in _parameters.Values)
            {
                if (parameter.Name == _debugParamName)
                    continue;//we never show debug since its for internal dev only

                string s = "-" + parameter.Name;
                while (s.Length < len + 3)
                    s += " ";
                if (parameter.Required == Required.Yes)
                    s += "[Required] ";
                else
                    s += "[Optional] ";
                if (parameter.AllowEmptyValue == AllowEmpty.Yes)
                    s += "[EmptyValueAllowed] ";
                s += parameter.Help + Environment.NewLine;
                help += s;
            }
            return help;
        }

        public IEnumerator<CmdLineParam> GetEnumerator()
        {
            return _parameters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _parameters.Count;
        public ValueMode ValueMode { get; }


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

                    _parameters[key].SetValue(value.Trim('|'), ValueMode);
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

        /// <summary>
        /// Adds a parameter named "debug". Not required and allows empty value. Makes the program wait for input from user so you can attach debugger to console app.
        /// usage [console.exe] -debug
        /// </summary>
        /// <returns></returns>
        private void RegisterDebug()
        {
            Register("debug", Required.No, AllowEmpty.Yes, param =>
            {
                if (param.Exists == false)
                    return;
                Console.WriteLine("In debug mode - attach debugger and press a key to continue");
                Console.ReadKey();
                Console.WriteLine("Continuing..");
            });
        }
    }
}
