using System.Collections.Generic;
using DotNet.Basics.AppSettings;
using DotNet.Basics.Collections;
using SimpleInjector;

namespace DotNet.Basics.Ioc
{
    public class SimpleContainer : Container, IAppSettingsProvider
    {
        private readonly AppSettingsProvider _settingsProvider;

        public SimpleContainer()
        {
            _settingsProvider = new AppSettingsProvider();
        }

        public void Register(params ISimpleRegistrations[] registrations)
        {
            registrations?.ForEach(r => r.RegisterIn(this));
        }

        public void RegisterAppSettings<T>(T appSettings) where T : class, IAppSettingsProvider
        {
            RegisterAppSettings<T>(appSettings, Lifestyle.Transient);
        }
        public void RegisterAppSettings<T>(T appSettings, Lifestyle lifestyle) where T : class, IAppSettingsProvider
        {
            Register<T>(() => appSettings, lifestyle);
            foreach (var appSetting in appSettings.GetAppSettings())
                _settingsProvider.Register(appSetting);
        }

        public new void Verify()
        {
            base.Verify();
            _settingsProvider.Verify();
        }
        public new void Verify(VerificationOption vo)
        {
            base.Verify(vo);
            _settingsProvider.Verify();
        }

        public IReadOnlyCollection<IAppSetting> GetAppSettings()
        {
            return _settingsProvider.GetAppSettings();
        }
    }
}
