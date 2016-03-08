namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public interface IInterfaceUsedInConstructor
    {
    }

    public class ClassImplementingInterfaceUsedInConstructor : IInterfaceUsedInConstructor
    {
    }

    public interface IInterfaceForClassDependentOnInterfaceUsedInConstructor
    {
    }

    public class ClassDependentOnInterfaceUsedInConstructor : IInterfaceForClassDependentOnInterfaceUsedInConstructor
    {
        public ClassDependentOnInterfaceUsedInConstructor(IInterfaceUsedInConstructor internalInterfaceUsedInConstructor)
        {

        }
    }
}
