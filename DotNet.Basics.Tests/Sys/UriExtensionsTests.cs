using System;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class UriExtensionsTests
    {
        const string _localhost = "https://localhost:50505/";
        const string _externalhost = "https://dr.dk:50505/";
        const string _pathAndQuery1 = "/My/Path/1/1/1/1?sdf=sdf&werwerwerw=2342342";
        const string _pathAndQuery2 = "/My/2/2/2/2?My=param&And=Something";
        [Test]
        public void HostUriOnly_StripHost_HostIsPreserved()
        {
            //arrange
            var rawUrl = _localhost.UrlCombine(_pathAndQuery1);
            var uri = new Uri(rawUrl);

            //act
            var strippedUri = uri.HostUriOnly();

            //assert
            uri.ToString().Should().Be(_localhost.UrlCombine(_pathAndQuery1));
            strippedUri.ToString().Should().Be(_localhost);
        }

        [Test]
        public void ReplacePathAndQuery_Replace_PathAndQueryIsCompletelyReplaced()
        {
            //arrange
            var oldUri = new Uri(_localhost.UrlCombine(_pathAndQuery1));

            //act
            var newUri = oldUri.ReplacePathAndQuery(_pathAndQuery2);

            //assert
            newUri.ToString().Should().Be(_localhost.UrlCombine(_pathAndQuery2));
        }


        [Test]
        public void ReplaceHost_ReplaceHost_HostIsUpdated()
        {
            //arrange
            var uri = new Uri(_localhost.UrlCombine(_pathAndQuery1));

            //act
            var newUri = uri.ReplaceHost(_externalhost);

            //assert
            newUri.ToString().Should().Be(_externalhost.UrlCombine(_pathAndQuery1));
        }

        [Test]
        public void EnsurePathPrefix_AddPrefixToPath_UriIsUpdated()
        {
            //arrange
            const string prefix = "MyPrefix";
            var uri = new Uri(_localhost.UrlCombine(_pathAndQuery1));

            //act
            var newUri = uri.EnsurePathPrefix(prefix);

            //assert
            newUri.ToString().Should().Be(_localhost.UrlCombine(prefix, _pathAndQuery1));
        }

        [Test]
        public void EnsurePathPrefix_PrefixIsAlreadyAdded_UriIsSame()
        {
            //arrange
            const string prefix = "MyPrefix";
            var uri = new Uri(_localhost.UrlCombine(prefix, _pathAndQuery1));

            //act
            var newUri = uri.EnsurePathPrefix(prefix);

            //assert
            newUri.ToString().Should().Be(_localhost.UrlCombine(prefix, _pathAndQuery1));
        }


        [Test]
        public void RemovePathPrefix_RemovePrefixToPath_UriIsUpdated()
        {
            //arrange
            const string prefix = "MyPrefix";
            var uri = new Uri(_localhost.UrlCombine(prefix, _pathAndQuery1));

            //act
            var newUri = uri.RemovePathPrefix(prefix);

            //assert
            newUri.ToString().Should().Be(_localhost.UrlCombine(_pathAndQuery1));
        }



        [Test]
        public void UrlCombine_FullHostAndPathWithSeperator_PathIsCombined()
        {
            var combined = "http://localhost/".UrlCombine("folder");
            combined.Should().Be("http://localhost/folder");
        }

        [Test]
        public void UrlCombine_FullHostAndPathWithoutSeparator_PathIsCombined()
        {
            var combined = "http://localhost".UrlCombine("folder");
            combined.Should().Be("http://localhost/folder");
        }
        [Test]
        public void UrlCombine_FullHostAndPathWithoutSeparatorWithEndSeparator_PathIsCombined()
        {
            var combined = "http://localhost".UrlCombine("/folder/");
            combined.Should().Be("http://localhost/folder/");
        }
    }
}
