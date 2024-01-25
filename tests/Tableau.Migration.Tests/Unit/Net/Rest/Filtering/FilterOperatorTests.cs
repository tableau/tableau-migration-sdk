using System;
using Tableau.Migration.Net.Rest.Filtering;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Filtering
{
    public class FilterOperatorTests
    {
        public abstract class FilterOperatorTest : AutoFixtureTestBase
        {
            protected static void AssertValue(FilterOperator @operator, string expectedValue)
            {
                Assert.Equal(expectedValue, @operator.Value);
            }
        }

        public class Ctor : FilterOperatorTest
        {
            [Fact]
            public void Sets_Value()
            {
                var value = Create<string>();

                var @operator = new FilterOperator(value);

                AssertValue(@operator, value);
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Throws_on_null_empty_or_whitespace_value(string? value)
            {
                var exception = Assert.Throws<ArgumentException>(() => new FilterOperator(value!));

                Assert.Equal("value", exception.ParamName);
            }
        }

        public class ToStringOverride : FilterOperatorTest
        {
            [Fact]
            public void Returns_Value()
            {
                var value = Create<string>();

                var @operator = new FilterOperator(value);

                Assert.Equal(value, @operator.ToString());
            }
        }

        public class DefinedOperators : FilterOperatorTest
        {
            [Fact]
            public void Returns_expected_value()
            {
                AssertValue(FilterOperator.Equal, "eq");
                AssertValue(FilterOperator.CaseInsensitiveEqual, "cieq");
                AssertValue(FilterOperator.In, "in");
                AssertValue(FilterOperator.GreaterThan, "gt");
                AssertValue(FilterOperator.GreaterThanOrEqual, "gte");
                AssertValue(FilterOperator.LessThan, "lt");
                AssertValue(FilterOperator.LessThanOrEqual, "lte");
                AssertValue(FilterOperator.Has, "has");
            }
        }
    }
}
