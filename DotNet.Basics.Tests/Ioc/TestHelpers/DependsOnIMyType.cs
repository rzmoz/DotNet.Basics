namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class DependsOnIMyType
    {
        public DependsOnIMyType(IMyType myType)
        {
            MyType = myType;
        }

        public IMyType MyType { get; private set; }

        public int GetValueFromIMyType()
        {
            return MyType.GetValue();
        }
    }
}
