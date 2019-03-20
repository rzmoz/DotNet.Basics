using System.Collections;
using System.Collections.Generic;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public class SwitchMappings : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _switchMappings = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            _switchMappings.Add(key.EnsurePrefix(CliArgsBuilder.MicrosoftExtensionsArgsSwitch), value);
        }

        public void Clear()
        {
            _switchMappings.Clear();
        }

        public IDictionary<string, string> ToDictionary()
        {
            return _switchMappings;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _switchMappings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
