using System;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class IProjectTypeExtensionsTests
    {
        public class GetControllingPermissionsProjectId
        {
            [Theory]
            [NullableGuidParseData]
            public void ParsesNullableGuid(string? s, Guid? expected)
            {
                var mockProject = new Mock<IProjectType>();
                mockProject.SetupGet(p => p.ControllingPermissionsProjectId).Returns(s);

                Assert.Equal(expected, mockProject.Object.GetControllingPermissionsProjectId());
            }
        }

        public class GetParentProjectId
        {
            [Theory]
            [NullableGuidParseData]
            public void ParsesNullableGuid(string? s, Guid? expected)
            {
                var mockProject = new Mock<IProjectType>();
                mockProject.SetupGet(p => p.ParentProjectId).Returns(s);

                Assert.Equal(expected, mockProject.Object.GetParentProjectId());
            }
        }
    }
}
