using System;
using DotNet.Basics.Rest;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    public class UriTests
    {
        [Fact]
        public void Ctor_FullUri_AllElementsAreSplit()
        {
            const string absoluteUri = "https://my.domain.dk:1010/folder/file?myParam=yes&nextParam=somethingElse";
            var uri = new Uri(absoluteUri);
            uri.AbsoluteUri.Should().Be(absoluteUri);
            uri.AbsolutePath.Should().Be("/folder/file");
            uri.Authority.Should().Be("my.domain.dk:1010");
            uri.Host.Should().Be("my.domain.dk");
            uri.PathAndQuery.Should().Be("/folder/file?myParam=yes&nextParam=somethingElse");
            uri.Port.Should().Be(1010);
            uri.Query.Should().Be("?myParam=yes&nextParam=somethingElse");
            uri.Scheme.Should().Be("https");
        }

        [Fact]
        public void BaseUri_GetBaseUri_BaseUriIsParsed()
        {
            const string absoluteUri = "https://my.domain.dk:1010/folder/file?myparam=yes&nextParam=somethingElse";
            var uri = new Uri(absoluteUri);
            uri.BaseUri().Should().Be("https://my.domain.dk:1010/");
        }
    }
}
