using System;
using System.Collections;
using System.Collections.Generic;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public class ArgsSwitchMappings : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _switchMappings = new Dictionary<string, string>();

        public ArgsSwitchMappings(Action<ArgsSwitchMappings> addSwitchMappings = null)
        {
            addSwitchMappings?.Invoke(this);
        }

        public void Add(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _switchMappings.Add(key.EnsurePrefix(ArgsExtensions.MicrosoftExtensionsArgsSwitch), value);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> mappings)
        {
            if (mappings == null)
                return;
            foreach (var mapping in mappings)
                Add(mapping.Key, mapping.Value);
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
