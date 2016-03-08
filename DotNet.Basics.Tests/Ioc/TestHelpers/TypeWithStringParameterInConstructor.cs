namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class TypeWithStringParameterInConstructor : ITypeWithStringParameterInConstructor
    {
        public TypeWithStringParameterInConstructor(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
