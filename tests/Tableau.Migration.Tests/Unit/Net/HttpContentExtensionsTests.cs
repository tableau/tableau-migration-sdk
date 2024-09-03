//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpContentExtensionsTests
    {
        public class IsUtf8Content
        {
            [Fact]
            public void NullContentType()
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = null;

                Assert.False(content.IsUtf8Content());
            }

            [Theory]
            [InlineData("utf-8")]
            [InlineData("UTF-8")]
            public void True(string charSet)
            {
                var content = new StringContent(charSet);
                content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml)
                {
                    CharSet = charSet
                };

                Assert.True(content.IsUtf8Content());
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("utf-7")]
            [InlineData("us-ascii")]
            public void False(string? charSet)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Xml)
                {
                    CharSet = charSet
                };

                Assert.False(content.IsUtf8Content());
            }
        }

        public class IsHtmlContent
        {
            [Fact]
            public void NullContentType()
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = null;

                Assert.False(content.IsHtmlContent());
            }

            [Theory]
            [InlineData("text/html")]
            [InlineData("TEXT/HTML")]
            public void True(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.True(content.IsHtmlContent());
            }

            [Theory]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            public void False(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.False(content.IsHtmlContent());
            }
        }

        public class IsJsonContent
        {
            [Fact]
            public void NullContentType()
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = null;

                Assert.False(content.IsJsonContent());
            }

            [Theory]
            [InlineData("application/json")]
            [InlineData("APPLICATION/JSON")]
            public void True(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.True(content.IsJsonContent());
            }

            [Theory]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            public void False(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.False(content.IsJsonContent());
            }
        }

        public class IsXmlContent
        {
            [Fact]
            public void NullContentType()
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = null;

                Assert.False(content.IsXmlContent());
            }

            [Theory]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            public void True(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.True(content.IsXmlContent());
            }

            [Theory]
            [InlineData("application/json")]
            [InlineData("APPLICATION/JSON")]
            public void False(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.False(content.IsXmlContent());
            }
        }

        public class IsTextContent
        {
            [Fact]
            public void NullContentType()
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = null;

                Assert.False(content.IsTextContent());
            }

            [Theory]
            [InlineData("text/Html")]
            [InlineData("text/xml")]
            [InlineData("text/json")]
            [InlineData("text/custom")]
            public void True(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.True(content.IsTextContent());
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
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.False(content.IsTextContent());
            }
        }

        public class LogsAsTextContent
        {
            [Fact]
            public void NullContentType()
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = null;

                Assert.False(content.LogsAsTextContent());
            }

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
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.True(content.LogsAsTextContent());
            }

            [Theory]
            [InlineData("application/pdf")]
            [InlineData("APPLICATION/octo-stream")]
            public void False(string mediaType)
            {
                var content = new StringContent("utf-8");
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                Assert.False(content.LogsAsTextContent());
            }
        }

        public class ReadAsEncodedStringAsync
        {
            [Fact]
            public async Task SupportsUtf8CharSet()
            {
                const string HTML = @"<html><head>
                          <title>404 Not Found</title>
                          </head><body>
                          <h1>Not Found</h1>
                          <p>The requested URL /refresh_extracts/workbooks was not found on this server.</p>
                          <p>Additionally, a 404 Not Found
                          error was encountered while trying to use an ErrorDocument to handle the request.</p>
                          </body></html>";

                var utf8 = new ByteArrayContent(Encoding.UTF8.GetBytes(HTML));

                utf8.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain)
                {
                    CharSet = "utf-8"
                };

                using var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = utf8
                };

                // act / assert does not throw
                var msg = await response.Content.ReadAsEncodedStringAsync(default);

                Assert.Equal(HTML, msg);
            }
        }
    }
}
