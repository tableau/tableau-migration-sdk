using System.Net.Http.Headers;
using System.Net.Mime;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class MediaTypeHeaderValueExtensionsTests
    {
        public class IsUtf8
        {
            [Theory]
            [InlineData("utf-8")]
            [InlineData("UTF-8")]
            public void True(string charSet)
            {
                var header = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml)
                {
                    CharSet = charSet
                };

                Assert.True(header.IsUtf8());
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("utf-7")]
            [InlineData("us-ascii")]
            public void False(string? charSet)
            {
                var header = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml)
                {
                    CharSet = charSet
                };

                Assert.False(header.IsUtf8());
            }
        }

        public class IsJson
        {
            [Theory]
            [InlineData("application/json")]
            [InlineData("APPLICATION/JSON")]
            public void True(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.True(header.IsJson());
            }

            [Theory]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            public void False(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.False(header.IsJson());
            }
        }

        public class IsXml
        {
            [Theory]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            public void True(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.True(header.IsXml());
            }

            [Theory]
            [InlineData("application/json")]
            [InlineData("APPLICATION/JSON")]
            public void False(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.False(header.IsXml());
            }
        }

        public class IsText
        {
            [Theory]
            [InlineData("text/Html")]
            [InlineData("text/xml")]
            [InlineData("text/json")]
            [InlineData("text/custom")]
            public void True(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.True(header.IsText());
            }

            [Theory]
            [InlineData("application/pdf")]
            [InlineData("APPLICATION/octo-stream")]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            [InlineData("application/json")]
            [InlineData("APPLICATION/JSON")]
            [InlineData("application/javascript")]
            [InlineData("application/ecmascript")]
            [InlineData("APPLICATION/Rtf")]
            [InlineData("application/sql")]
            [InlineData("application/typescript")]
            [InlineData("application/xhtml+xml")]
            [InlineData("application/x-www-form-urlencoded")]
            [InlineData("application/graphql")]
            public void False(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.False(header.IsText());
            }
        }

        public class LogsAsText
        {
            [Theory]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            [InlineData("application/json")]
            [InlineData("APPLICATION/JSON")]
            [InlineData("application/javascript")]
            [InlineData("application/ecmascript")]
            [InlineData("APPLICATION/Rtf")]
            [InlineData("application/sql")]
            [InlineData("application/typescript")]
            [InlineData("application/xhtml+xml")]
            [InlineData("application/x-www-form-urlencoded")]
            [InlineData("application/graphql")]
            [InlineData("text/Html")]
            [InlineData("text/xml")]
            [InlineData("text/json")]
            [InlineData("text/custom")]
            public void True(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.True(header.LogsAsText());
            }

            [Theory]
            [InlineData("application/pdf")]
            [InlineData("APPLICATION/octo-stream")]
            public void False(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.False(header.LogsAsText());
            }
        }
    }
}
