using System;
using System.Linq;
using System.Reflection;

namespace DotNet.Basics.Cli
{
    public class PipelineArgsFactory
    {
        public object Create(Type pipelineType, ArgsProvider args)
        {
            var argsPipelineType = pipelineType;

            while (argsPipelineType!.GetGenericArguments().Any() == false)
                argsPipelineType = argsPipelineType.BaseType;


            var argsType = argsPipelineType.GetGenericArguments().Single();
            var argsObject = Activator.CreateInstance(argsType)!;
            var argsProperties = argsType.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            foreach (var argsProperty in argsProperties)
            {
                if (!argsProperty.CanWrite)
                    continue;

                if (argsProperty.PropertyType == typeof(int))
                    argsProperty.SetValue(argsObject, int.Parse(args[argsProperty.Name]));
                else if (argsProperty.PropertyType == typeof(double))
                    argsProperty.SetValue(argsObject, double.Parse(args[argsProperty.Name]));
                else if (argsProperty.PropertyType == typeof(bool))
                    argsProperty.SetValue(argsObject, bool.Parse(args[argsProperty.Name]));
                else
                    argsProperty.SetValue(argsObject, args[argsProperty.Name]);
            }

            return argsObject;
        }
    }
}

