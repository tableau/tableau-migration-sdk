//
//  Copyright (c) 2025, Salesforce, Inc.
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

using System;
using Tableau.Migration.Net.Logging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Logging
{
    public class HttpContentRedactorTests
    {
        public class ReplaceSensitiveData
        {
            private readonly HttpContentRedactor _httpContentRedactor;

            public ReplaceSensitiveData()
            {
                _httpContentRedactor = new HttpContentRedactor();
            }

            [Fact]
            public void NonSensitiveAttribute()
            {
                var input = "firstkey blah=\"test\" anotherkey";

                Assert.Equal(input, _httpContentRedactor.ReplaceSensitiveData(input));
            }

            [Fact]
            public void Empty()
            {
                var input = string.Empty;

                var output = _httpContentRedactor.ReplaceSensitiveData(input);
                Assert.Equal(input, output);
                Assert.Empty(output);
            }

            [Fact]
            public void NonSensitiveElement()
            {
                var input = "firstkey <name>test</name> anotherkey";

                Assert.Equal(input, _httpContentRedactor.ReplaceSensitiveData(input));
            }

            [Theory]
            [InlineData("authenticity_token", "test")]
            [InlineData("modulus", "test")]
            [InlineData("exponent", "test")]
            [InlineData("authenticity_token", "modulus")]
            [InlineData("authenticity_token", "exponent")]
            [InlineData("modulus", "authenticity_token")]
            [InlineData("modulus", "exponent")]
            [InlineData("exponent", "authenticity_token")]
            [InlineData("exponent", "modulus")]
            public void IncorrectElementTags(
                string startTag,
                string endTag)
            {
                var input = $"firstkey <{startTag}>test</{endTag}> anotherkey";

                Assert.Equal(input, _httpContentRedactor.ReplaceSensitiveData(input));
            }

            [Theory]
            [InlineData("password", "test")]
            [InlineData("password", "te&quot;st")]
            [InlineData("password", "te'st")]
            [InlineData("password", "")]
            [InlineData("token", "test")]
            [InlineData("token", "te&quot;st")]
            [InlineData("token", "te'st")]
            [InlineData("token", "")]
            [InlineData("jwt", "test")]
            [InlineData("jwt", "te&quot;st")]
            [InlineData("jwt", "te'st")]
            [InlineData("jwt", "")]
            [InlineData("personalAccessTokenSecret", "test")]
            [InlineData("personalAccessTokenSecret", "te&quot;st")]
            [InlineData("personalAccessTokenSecret", "te'st")]
            [InlineData("personalAccessTokenSecret", "")]
            public void ReplaceXmlAttributes(
                string attributeName,
                string secret)
            {
                TestValue(
                    $"firstkey {attributeName}=\"{secret}\" anotherkey",
                    secret);
            }

            [Theory]
            [InlineData("authenticity_token", "test")]
            [InlineData("authenticity_token", "te\\\"st")]
            [InlineData("authenticity_token", "te\\'st")]
            [InlineData("authenticity_token", "")]
            [InlineData("modulus", "test")]
            [InlineData("modulus", "te\\\"st")]
            [InlineData("modulus", "te\\'st")]
            [InlineData("modulus", "")]
            [InlineData("exponent", "test")]
            [InlineData("exponent", "te\\\"st")]
            [InlineData("exponent", "te\\'st")]
            [InlineData("exponent", "")]
            public void ReplaceXmlElement(
                string elementName,
                string secret)
            {
                TestValue(
                    $"firstkey <{elementName}>{secret}</{elementName}> anotherkey",
                    secret);
            }

            [Theory]
            [InlineData("test")]
            [InlineData("te&quot;st")]
            [InlineData("te'st")]
            [InlineData("")]
            public void ReplaceMultipleSecrets(string secret)
            {
                TestValue(
                    $"firstkey " +
                    $"password=\"{secret}\" " +
                    $"token=\"{secret}\" " +
                    $"something " +
                    $"<authenticity_token>{secret}</authenticity_token> " +
                    $"<modulus>{secret}</modulus> " +
                    $"jwt=\"{secret}\" " +
                    $"personalAccessTokenSecret=\"{secret}\" " +
                    $"<exponent>{secret}</exponent> " +
                    $"anotherkey",
                    secret);
            }

            private void TestValue(
                string input,
                string sensitiveValue)
            {
                // Arrange
                var expectedResult = input;

                if (!sensitiveValue.IsNullOrEmpty())
                {
                    expectedResult = input.Replace(
                        sensitiveValue,
                        HttpContentRedactor.SENSITIVE_DATA_PLACEHOLDER,
                        StringComparison.Ordinal);
                }

                // Act
                var result = _httpContentRedactor.ReplaceSensitiveData(input);
                var resultUpperCase = _httpContentRedactor.ReplaceSensitiveData(input.ToUpper());

                // Assert
                Assert.Equal(expectedResult, result);
                Assert.Equal(expectedResult, resultUpperCase, true);
            }
        }

        public class IsSensitiveMultipartContent
        {
            private readonly HttpContentRedactor _httpContentRedactor;

            public IsSensitiveMultipartContent()
            {
                _httpContentRedactor = new HttpContentRedactor();
            }

            [Fact]
            public void EmptyName()
            {
                Assert.False(_httpContentRedactor.IsSensitiveMultipartContent(string.Empty));
            }

            [Fact]
            public void NullName()
            {
                Assert.False(_httpContentRedactor.IsSensitiveMultipartContent(null));
            }

            [Theory]
            [InlineData("readonly-password")]
            [InlineData("auth-token")]
            [InlineData("authenticity_token_renew")]
            [InlineData("full_keychain_key")]
            [InlineData("crypted content")]
            public void SensitiveMultipartContentName(
                string input)
            {
                Assert.True(_httpContentRedactor.IsSensitiveMultipartContent(input));
            }

            [Theory]
            [InlineData("filename")]
            [InlineData("body")]
            [InlineData("test-content")]
            [InlineData("payload")]
            [InlineData("dummy data")]
            public void NotSensitiveMultipartContentName(
                string input)
            {
                Assert.False(_httpContentRedactor.IsSensitiveMultipartContent(input));
            }
        }
    }
}
