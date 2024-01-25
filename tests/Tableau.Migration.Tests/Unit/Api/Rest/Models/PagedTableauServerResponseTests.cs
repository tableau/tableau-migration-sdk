using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class PagedTableauServerResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes_error()
            {
                var expectedError = Create<Error>();

                var xml = $@"
<tsResponse>
    <error code=""{expectedError.Code}"">
        <summary>{expectedError.Summary}</summary>
        <detail>{expectedError.Detail}</detail>
    </error>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<TestPagedTableauServerResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.NotNull(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);

                Assert.Equal(expectedError.Code, deserialized.Error.Code);
                Assert.Equal(expectedError.Summary, deserialized.Error.Summary);
                Assert.Equal(expectedError.Detail, deserialized.Error.Detail);
            }

            [Fact]
            public void Deserializes_pagination()
            {
                var expectedPagination = Create<Pagination>();

                var xml = $@"
<tsResponse>
    <pagination pageNumber=""{expectedPagination.PageNumber}"" pageSize=""{expectedPagination.PageSize}"" totalAvailable=""{expectedPagination.TotalAvailable}""/>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<TestPagedTableauServerResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);

                Assert.Equal(expectedPagination.PageNumber, deserialized.Pagination.PageNumber);
                Assert.Equal(expectedPagination.PageSize, deserialized.Pagination.PageSize);
                Assert.Equal(expectedPagination.TotalAvailable, deserialized.Pagination.TotalAvailable);
            }
        }
    }
}