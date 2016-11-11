using System.Collections.Generic;
using System.Linq;
using Autofac;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.AppSettings
{
    public static class IocAppSettingExtensions
    {
        public static IReadOnlyCollection<IAppSetting> GetAppSettings(this IContainer container)
        {
            return container?.Resolve<IEnumerable<IAppSetting>>()?.ToList();
        }

        public static AppSettingsVerificationResult VerifyRequiredAppSettingKeysAreConfigured(this IContainer container)
        {
            return container?.GetAppSettings()?.VerifyRequiredAppSettingKeysAreConfigured();
        }
        public static AppSettingsVerificationResult VerifyRequiredAppSettingKeysAreConfigured(this IEnumerable<IAppSetting> settings)
        {
            var missingKeys = new List<string>();
            foreach (var appSetting in settings)
            {
                if (appSetting.Verify() == false)
                    missingKeys.Add(appSetting.Key);
            }
            return new AppSettingsVerificationResult(missingKeys);
        }

        public static void Register(this IocBuilder builder, params IIocRegistrations[] iocRegistrations)
        {
            foreach (var iocRegistrationse in iocRegistrations)
                iocRegistrationse.RegisterIn(builder);
        }

        public static void RegisterAppSettings<T>(this ContainerBuilder builder, params T[] appSettings) where T : class, IAppSetting
        {
            foreach (var appSetting in appSettings)
                builder.RegisterInstance(appSetting).As<IAppSetting>().AsSelf().ExternallyOwned();
        }
    }
}
