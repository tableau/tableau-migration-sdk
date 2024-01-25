using System.Collections.Generic;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class QueryStringBuilderTests
    {
        public abstract class QueryStringBuilderTest : AutoFixtureTestBase
        {
            internal readonly QueryStringBuilder Builder = new();
        }

        public class AddOrUpdate : QueryStringBuilderTest
        {
            [Theory]
            [InlineData("aaa", "bbb", "aaa=bbb")]
            [InlineData("aaa bbb", "ccc", "aaa+bbb=ccc")]
            [InlineData("aaa & bbb", "ccc", "aaa+%26+bbb=ccc")]
            [InlineData("aaa & bbb", "ccc ddd", "aaa+%26+bbb=ccc+ddd")]
            [InlineData("aaa & bbb", "ccc & ddd", "aaa+%26+bbb=ccc+%26+ddd")]
            [InlineData("aaa\nbbb", "ccc\nddd", "aaa%0Abbb=ccc%0Addd")]
            [InlineData(" \t\r\n\0~!@#$%^&*()_+{}|:\"<>?`[]\\;',./=", " \t\r\n\0~!@#$%^&*()_+{}|:\"<>?`[]\\;',./=", "+%09%0D%0A%00%7E!%40%23%24%25%5E%26*()_%2B%7B%7D%7C%3A%22%3C%3E%3F%60%5B%5D%5C%3B%27%2C.%2F%3D=+%09%0D%0A%00%7E!%40%23%24%25%5E%26*()_%2B%7B%7D%7C%3A%22%3C%3E%3F%60%5B%5D%5C%3B%27%2C.%2F%3D")]
            public void Encodes(string key, string value, string expected)
            {
                // key, value overload
                {
                    Builder.AddOrUpdate(key, value);

                    var query = Builder.Build();

                    Assert.Equal(expected, query);
                }

                // IDictionary overload
                {
                    Builder.AddOrUpdate(new Dictionary<string, string>
                    {
                        [key] = value
                    });

                    var query = Builder.Build();

                    Assert.Equal(expected, query);
                }
            }
        }
    }
}
