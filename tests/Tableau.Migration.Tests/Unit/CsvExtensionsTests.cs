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

using System;
using System.Text;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class CsvExtensionsTests
    {
        public class AppendCsvLine
        {
            [Fact]
            public void BuildsCsvFromString()
            {
                var test = new StringBuilder();
                var result = test.AppendCsvLine("One", "Two", "Three");

                Assert.Same(result, test);
                Assert.Equal($"One,Two,Three{Environment.NewLine}", test.ToString());
            }

            [Fact]
            public void NoLeadingOrTrailingComma()
            {
                var test = new StringBuilder();
                test.AppendCsvLine("first", "second");

                Assert.False(test.ToString().StartsWith(','));
                Assert.False(test.ToString().TrimEnd(Environment.NewLine.ToCharArray()).EndsWith(','));
            }

            [Fact]
            public void EmptyAndNullValues()
            {
                var test = new StringBuilder();
                test.AppendCsvLine("first", "", null, "second");
                Assert.Equal($"first,,,second{Environment.NewLine}", test.ToString());
            }
        }

        public class AppendCsvValue
        {
            [Theory]
            [InlineData("", "")]
            [InlineData("test,", "\"test,\"")]
            [InlineData("test,test", "\"test,test\"")]
            [InlineData("test\n", "\"test\n\"")]
            [InlineData("William O\"Connor", "\"William O\"\"Connor\"")]
            [InlineData("test\nall\runsafe\r\ncharacters\"", "\"test\nall\runsafe\r\ncharacters\"\"\"")]
            public void Escapes(string input, string expectedOutput)
            {
                var test = new StringBuilder();
                test.AppendCsvValue(input);
                Assert.Equal(expectedOutput, test.ToString());
            }
        }
    }
}
