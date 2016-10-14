namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class ClassThatIncrementArgsDependOn
    {
        public int IncrementByOne(int input)
        {
            return ++input;
        }
    }
}
