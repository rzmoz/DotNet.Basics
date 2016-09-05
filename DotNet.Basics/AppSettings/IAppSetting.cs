namespace DotNet.Basics.AppSettings
{
    public interface IAppSetting
    {
        string Key { get; }
        bool Required { get; }
        bool Verify();
    }
}
