using System;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class SiteTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            protected SiteResponse CreateTestResponse()
            {
                return new SiteResponse
                {
                    Item = new()
                    {
                        Id = Create<Guid>(),
                        Name = Create<string>(),
                        ContentUrl = Create<string>()
                    }
                };
            }

            [Fact]
            public void Initializes()
            {
                var response = CreateTestResponse();
                var site = new Site(response);

                Assert.Equal(response.Item!.Id, site.Id);
                Assert.Equal(response.Item.Name, site.Name);
                Assert.Equal(response.Item.ContentUrl, site.ContentUrl);
            }

            [Fact]
            public void ItemRequired()
            {
                var response = CreateTestResponse();
                response.Item = null;

                Assert.Throws<ArgumentNullException>(() => new Site(response));
            }

            [Fact]
            public void EmptyId()
            {
                var response = CreateTestResponse();
                response.Item!.Id = Guid.Empty;

                Assert.Throws<ArgumentException>(() => new Site(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void SiteNameRequired(string? name)
            {
                var response = CreateTestResponse();
                response.Item!.Name = name;

                Assert.Throws<ArgumentException>(() => new Site(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void ContentUrlShouldNotBeNull(string? contentUrl)
            {
                var response = CreateTestResponse();
                response.Item!.ContentUrl = contentUrl;

                if (contentUrl is null)
                {
                    Assert.Throws<ArgumentNullException>(() => new Site(response));
                }
                else
                {
                    var site = new Site(response);
                }
            }
        }
    }
}
