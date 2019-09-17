using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public interface ICliConfiguration
    {
        string this[string key, int index] { get; }
        string this[string key] { get; }
        string this[int index] { get; }

        IReadOnlyList<string> Args { get; }
        IConfigurationRoot Config { get; }
        IReadOnlyCollection<string> Environments { get; }

        bool IsSet(string key);
        bool HasValue(string key);
    }
}
