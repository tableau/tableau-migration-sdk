using System;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class MappableContainerContentBaseTests
    {
        public class TestContainerContentBase : MappableContainerContentBase
        {
            protected override IContentReference? MappableContainer { get; set; }
        }

        public class SetLocation
        {
            [Fact]
            public void SetsContainerAndName()
            {
                var c = new TestContainerContentBase();

                var parent = new ContentReferenceStub(Guid.NewGuid(), string.Empty, new("parent", "project"));
                var loc = parent.Location.Append("Name");

                ((IMappableContainerContent)c).SetLocation(parent, loc);

                Assert.Same(parent, ((IMappableContainerContent)c).Container);
                Assert.Equal(loc, c.Location);
                Assert.Equal("Name", c.Name);
            }
        }
    }
}
