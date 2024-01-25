using System;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class TagTests
    {
        public class FromTagType : AutoFixtureTestBase
        {
            protected ITagType CreateTestResponse()
            {
                return Create<ITagType>();
            }

            private static void AssertSuccess(ITagType? response, ITag result)
            {
                if (response == null)
                {
                    return;
                }

                Assert.NotNull(result);
                Assert.Equal(response.Label, result.Label);
            }

            [Fact]
            public void Success()
            {
                var response = CreateTestResponse();

                var result = new Tag(response);
                FromTagType.AssertSuccess(response, result);
            }


            [Fact]
            public void InvalidName()
            {
                var response = CreateTestResponse();
                response.Label = null;

                Assert.Throws<ArgumentException>(() => new Tag(response));
            }
        }
    }
}

