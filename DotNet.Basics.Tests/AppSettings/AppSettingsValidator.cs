using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.AppSettings;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.AppSettings
{
    [TestFixture]
    public class AppSettingsValidatorTests
    {
        [Test]
        public void Verify_RequiredKeys_RequiredKeysArePresent()
        {
            var validator = new AppSettingsProvider();
            validator.Register(new AppSetting<string>("RequiredKey"));

            Action action = () => validator.Verify();
            action.ShouldNotThrow();
        }
        [Test]
        public void Verify_RequiredKeys_RequiredKeysAreMissing()
        {
            var missingKey = "MissingKey";
            var validator = new AppSettingsProvider();
            validator.Register(new AppSetting<string>(missingKey));

            Action action = () => validator.Verify();
            action.ShouldThrow<RequiredConfigurationNotSetException>().WithMessage(missingKey);
        }

        [Test]
        public void Verify_DefaultValues_SettingsWithDefaultValuesAreNotRequiredToBeSet()
        {
            var defaultValue = "MyDefaultValue%&/%&/¤%/%&";
            var appSetting = new AppSetting<string>("MissingKey", defaultValue);
            var settingsValidator = new AppSettingsProvider();
            settingsValidator.Register(appSetting);
            Action action = () => settingsValidator.Verify();
            action.ShouldNotThrow();
            appSetting.GetValue().Should().Be(defaultValue);
            appSetting.DefaultValue.Should().Be(defaultValue);
        }
    }
}