namespace DotNet.Basics.AppSettings
{
    public interface IAppSettingsProvider
    {
        string Get(string key);
    }
}
