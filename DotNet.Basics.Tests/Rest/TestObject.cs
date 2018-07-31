namespace DotNet.Basics.Tests.Rest
{
    public class TestObject
    {
        public TestObject(string immutable)
        {
            Immutable = immutable;
        }

        public string Mutable{ get; set; }//public getter and setter
        public string Immutable { get; }//public getter and setter
    }
}
