using System;
using DotNet.Basics.AppSettings;
using DotNet.Basics.Ioc;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.AppSettings
{
    [TestFixture]
    public class IocAppSettingExtensionsTests
    {
        private IocBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new IocBuilder();
        }

        [Test]
        public void Verify_RequiredKeys_RequiredKeysArePresent()
        {
            _builder.RegisterAppSettings(new AppSetting<string>("RequiredKey"));
            var container = _builder.Build();
            Action action = () => container.VerifyAppSettings();
            action.ShouldNotThrow();
        }
        [Test]
        public void Verify_RequiredKeys_RequiredKeysAreMissing()
        {
            var missingKey = "MissingKey";
            _builder.RegisterAppSettings(new AppSetting<string>(missingKey + 1));
            _builder.RegisterAppSettings(new AppSetting<string>(missingKey + 2));
            var container = _builder.Build();
            Action action = () => container.VerifyAppSettings();
            action.ShouldThrow<RequiredConfigurationKeyNotSetException>().And.MissingKeys.Count.Should().Be(2);
        }

        [Test]
        public void Verify_DefaultValues_SettingsWithDefaultValuesAreNotRequiredToBeSet()
        {
            var defaultValue = "MyDefaultValue%&/%&/¤%/%&";
            var appSetting = new AppSetting<string>("MissingKey", defaultValue);
            _builder.RegisterAppSettings(appSetting);
            var container = _builder.Build();
            Action action = () => container.VerifyAppSettings();
            action.ShouldNotThrow();
            appSetting.GetValue().Should().Be(defaultValue);
            appSetting.DefaultValue.Should().Be(defaultValue);
        }
    }
}