namespace DotNet.Basics.Sys.Text;

public static class SysRegexExtensions
{
    public static string Remove(this string input, params SysRegex[] regexes)
    {
        foreach (var regex in regexes)
            input = regex.Remove(input);
        return input;
    }
}