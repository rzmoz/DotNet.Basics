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
            var settingsValidator = new AppSettingsValidator();
            settingsValidator.Register(new AppSetting<string>("RequiredKey"));
            settingsValidator.GetRequiredKeysNotSet().Any().Should().BeFalse();
        }
        [Test]
        public void Verify_RequiredKeys_RequiredKeysAreMissing()
        {
            var missingKey = "MissingKey";
            var settingsValidator = new AppSettingsValidator();
            settingsValidator.Register(new AppSetting<string>(missingKey));
            settingsValidator.GetRequiredKeysNotSet().Single().Should().Be(missingKey);
        }

        [Test]
        public void Verify_NotRequiredKeys_NotRequiredKeysArePresent()
        {
            var settingsValidator = new AppSettingsValidator();
            settingsValidator.Register(new AppSetting<string>("MissingKey", false, null));
            settingsValidator.GetRequiredKeysNotSet().Any().Should().BeFalse();
        }
    }
}