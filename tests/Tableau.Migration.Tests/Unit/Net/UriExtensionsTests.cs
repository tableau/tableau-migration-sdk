﻿using System;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class UriExtensionsTests
    {
        public abstract class UriExtensionsTest : AutoFixtureTestBase
        { }

        public class IsRest : UriExtensionsTest
        {
            [Theory]
            [InlineData("api/1.0")]
            [InlineData("/api/1.0")]
            [InlineData("api/1.0/")]
            [InlineData("/api/1.0/")]
            [InlineData("https://localhost/api/1.0/")]
            [InlineData("https://localhost/api/1.0/path")]
            public void True(string url)
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);

                Assert.True(uri.IsRest());

                uri = new Uri(url.ToUpper(), UriKind.RelativeOrAbsolute);

                Assert.True(uri.IsRest());
            }

            [Theory]
            [InlineData(null)]
            [InlineData("api-i-am-not")]
            [InlineData("/api-i-am-not")]
            [InlineData("api-i-am-not/")]
            [InlineData("/api-i-am-not/")]
            [InlineData("/api-i-am-not/path")]
            [InlineData("https://localhost/api-i-am-not/")]
            [InlineData("https://localhost/api-i-am-not/path")]
            public void False(string? url)
            {
                var uri = url is not null ? new Uri(url, UriKind.RelativeOrAbsolute) : null;

                Assert.False(uri.IsRest());

                if (url is not null)
                {
                    uri = new Uri(url.ToUpper(), UriKind.RelativeOrAbsolute);

                    Assert.False(uri.IsRest());
                }
            }
        }

        public class IsRestSignIn : UriExtensionsTest
        {
            [Theory]
            [InlineData("api/1.0/auth/signin")]
            [InlineData("/api/1.0/auth/signin")]
            [InlineData("api/1.0/auth/signin/")]
            [InlineData("/api/1.0/auth/signin/")]
            [InlineData("https://localhost/api/1.0/auth/signin")]
            public void True(string url)
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);

                Assert.True(uri.IsRestSignIn());

                uri = new Uri(url.ToUpper(), UriKind.RelativeOrAbsolute);

                Assert.True(uri.IsRestSignIn());
            }

            [Theory]
            [InlineData(null)]
            [InlineData("api-i-am-not/1.0/auth/signin")]
            [InlineData("/api/1.0/auth/signout")]
            public void False(string? url)
            {
                var uri = url is not null ? new Uri(url, UriKind.RelativeOrAbsolute) : null;

                Assert.False(uri.IsRestSignIn());

                if (url is not null)
                {
                    uri = new Uri(url.ToUpper(), UriKind.RelativeOrAbsolute);

                    Assert.False(uri.IsRestSignIn());
                }
            }
        }

        public class IsRestSignOut : UriExtensionsTest
        {
            [Theory]
            [InlineData("api/1.0/auth/signout")]
            [InlineData("/api/1.0/auth/signout")]
            [InlineData("api/1.0/auth/signout/")]
            [InlineData("/api/1.0/auth/signout/")]
            [InlineData("https://localhost/api/1.0/auth/signout")]
            public void True(string url)
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);

                Assert.True(uri.IsRestSignOut());

                uri = new Uri(url.ToUpper(), UriKind.RelativeOrAbsolute);

                Assert.True(uri.IsRestSignOut());
            }

            [Theory]
            [InlineData(null)]
            [InlineData("api-i-am-not/1.0/auth/signin")]
            [InlineData("/api/1.0/auth/signin")]
            public void False(string? url)
            {
                var uri = url is not null ? new Uri(url, UriKind.RelativeOrAbsolute) : null;

                Assert.False(uri.IsRestSignOut());

                if (url is not null)
                {
                    uri = new Uri(url.ToUpper(), UriKind.RelativeOrAbsolute);

                    Assert.False(uri.IsRestSignOut());
                }
            }
        }
    }
}
