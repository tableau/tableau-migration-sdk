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
