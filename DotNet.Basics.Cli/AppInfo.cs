using System;
using System.Diagnostics;
using System.Reflection;

namespace DotNet.Basics.Cli
{
    public class AppInfo
    {
        public AppInfo(Type rootType = null)
        : this(rootType?.Assembly ?? Assembly.GetEntryAssembly())
        {
        }

        public AppInfo(Assembly rootAssembly)
        {
            if (rootAssembly == null)
                return;
            Name = rootAssembly.GetName().Name;
            Version = FileVersionInfo.GetVersionInfo(rootAssembly.Location).ProductVersion;
        }

        public string Name { get; }
        public string Version { get; }
    }
}
