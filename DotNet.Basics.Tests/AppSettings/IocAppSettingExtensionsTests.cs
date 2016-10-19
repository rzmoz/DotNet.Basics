using System.Linq;
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

            //act
            var result = _builder.Container.VerifyRequiredAppSettingKeysAreConfigured();

            result.AllGood.Should().BeTrue();
        }
        [Test]
        public void Verify_RequiredKeys_RequiredKeysAreMissing()
        {
            var missingKey = "MissingKey";
            _builder.RegisterAppSettings(new AppSetting<string>(missingKey + 1));
            _builder.RegisterAppSettings(new AppSetting<string>(missingKey + 2));

            //act
            var result = _builder.Container.VerifyRequiredAppSettingKeysAreConfigured();

            result.AllGood.Should().BeFalse();
            result.MissingKeys.Count.Should().Be(2);
            result.MissingKeys.First().Should().Be(missingKey + 1);
            result.MissingKeys.Last().Should().Be(missingKey + 2);
        }

        [Test]
        public void Verify_DefaultValues_SettingsWithDefaultValuesAreNotRequiredToBeSet()
        {
            var defaultValue = "MyDefaultValue%&/%&/¤%/%&";
            var appSetting = new AppSetting<string>("MissingKey", defaultValue);
            _builder.RegisterAppSettings(appSetting);

            //act
            var result = _builder.Container.VerifyRequiredAppSettingKeysAreConfigured();
            result.AllGood.Should().BeTrue();

            appSetting.GetValue().Should().Be(defaultValue);
            appSetting.DefaultValue.Should().Be(defaultValue);
        }
    }
}