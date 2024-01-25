using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    internal class AddTagsRequestTests
    {
        public class TagType
        {
            public class LabelQuoting
            {
                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData("text")]
                [InlineData("a tag")]
                public void RoundtripsQuotedLabel(string? label)
                {
                    var mockTag = new Mock<ITagType>();
                    mockTag.Setup(x => x.Label).Returns(label);

                    var t = new AddTagsRequest.TagType(mockTag.Object);

                    Assert.Equal(label, t.Label);
                    if (label.IsNullOrEmpty())
                    {
                        Assert.Equal(label, t.QuotedLabel);
                    }
                    else
                    {
                        Assert.Equal($"\"{label}\"", t.QuotedLabel);
                    }
                }
            }
        }
    }
}
