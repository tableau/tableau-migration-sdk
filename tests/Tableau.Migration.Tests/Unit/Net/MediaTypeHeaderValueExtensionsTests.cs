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

        public class IsHtml
        {
            [Theory]
            [InlineData("text/html")]
            [InlineData("TEXT/HTML")]
            public void True(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.True(header.IsHtml());
            }

            [Theory]
            [InlineData("application/xml")]
            [InlineData("APPLICATION/XML")]
            public void False(string mediaType)
            {
                var header = new MediaTypeHeaderValue(mediaType);

                Assert.False(header.IsHtml());
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
