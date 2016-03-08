namespace DotNet.Basics.Tests.Pipelines
{
    public class ClassThatIncrementArgsDependOn
    {
        public int IncrementByOne(int input)
        {
            return ++input;
        }
    }
}
