using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.AppSettings;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.AppSettings
{
    [TestFixture]
    public class AppSettingsProviderTests
    {
        [Test]
        public void Verify_RequiredKeys_RequiredKeysArePresent()
        {
            var settingsProvider = new AppSettingsProvider();
            settingsProvider.Register(new AppSetting<string>("RequiredKey"));

            //act
            var isValid = settingsProvider.VerifyAll();

            isValid.Should().BeTrue();
        }

        [Test]
        public void Verify_RequiredKeys_RequiredKeysAreMissing()
        {
            var missingKey = "MissingKey";
            var settingsProvider = new AppSettingsProvider();
            settingsProvider.Register(new AppSetting<string>(missingKey));
            IReadOnlyCollection<string> missingKeys;

            //act
            var isValid = settingsProvider.VerifyAll(out missingKeys);

            isValid.Should().BeFalse();
            missingKeys.Single().Should().Be(missingKey);
        }

        [Test]
        public void Verify_DefaultValues_SettingsWithDefaultValuesAreNotRequiredToBeSet()
        {
            //arrange
            var defaultValue = "MyDefaultValue%&/%&/¤%/%&";
            var appSetting = new AppSetting<string>("MissingKey", defaultValue);
            var settingsValidator = new AppSettingsProvider();
            settingsValidator.Register(appSetting);

            //act
            var isValid = settingsValidator.VerifyAll();

            //assert
            isValid.Should().BeTrue();
            appSetting.GetValue().Should().Be(defaultValue);
            appSetting.DefaultValue.Should().Be(defaultValue);
        }
    }
}