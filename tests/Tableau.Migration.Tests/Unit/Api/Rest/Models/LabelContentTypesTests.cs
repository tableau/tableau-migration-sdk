using System;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class LabelContentTypesTests
    {
        public class FromContentType
        {
            [Theory]
            [InlineData(typeof(IDataSource), LabelContentTypes.DataSource)]
            [InlineData(typeof(IPublishableDataSource), LabelContentTypes.DataSource)]
            public void Parses(Type inputContentType, string expectedResult)
                => Assert.Equal(expectedResult, LabelContentTypes.FromContentType(inputContentType));

            [Theory]
            [InlineData(typeof(IProject))]
            public void Throws_exception_when_unsupported(Type inputContentType)
                => Assert.Throws<NotSupportedException>(() => LabelContentTypes.FromContentType(inputContentType));
        }
    }
}
