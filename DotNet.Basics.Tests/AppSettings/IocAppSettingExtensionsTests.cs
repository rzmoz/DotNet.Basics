using System.Linq;
using DotNet.Basics.AppSettings;
using DotNet.Basics.Ioc;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.AppSettings
{
    public class IocAppSettingExtensionsTests
    {
        private readonly IocBuilder _builder;
        
        public IocAppSettingExtensionsTests()
        {
            _builder = new IocBuilder();
        }

        [Fact]
        public void Verify_RequiredKeys_RequiredKeysArePresent()
        {
            _builder.RegisterAppSettings(new AppSetting<string>("RequiredKey"));

            //act
            var result = _builder.Container.VerifyRequiredAppSettingKeysAreConfigured();

            result.AllGood.Should().BeTrue();
        }
        [Fact]
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

        [Fact]
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