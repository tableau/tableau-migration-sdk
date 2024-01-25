using Tableau.Migration.Net.Rest.Fields;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest.Fields
{
    public class FieldTests
    {
        public abstract class FieldTest : AutoFixtureTestBase
        {
            protected static void AssertValue(Field field, string expectedName)
            {
                Assert.Equal(expectedName, field.Name);
            }
        }

        public class Ctor : FieldTest
        {
            [Fact]
            public void Sets_Name()
            {
                var name = Create<string>();

                var field = new Field(name);

                Assert.Equal(name, field.Name);
            }

            [Fact]
            public void Sets_Expression()
            {
                var name = Create<string>();

                var field = new Field(name);

                Assert.Equal(name, field.Expression);
            }
        }

        public class DefinedFields : FieldTest
        {
            [Fact]
            public void Returns_expected_value()
            {
                AssertValue(Field.All, "_all_");
                AssertValue(Field.Default, "_default_");
            }
        }
    }
}
