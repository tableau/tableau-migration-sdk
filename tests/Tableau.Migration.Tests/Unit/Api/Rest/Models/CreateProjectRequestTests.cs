using System;
using AutoFixture;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class CreateProjectRequestTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var request = AutoFixture.Create<CreateProjectRequest>();

                Assert.NotNull(request.Project);

                request.Project.ParentProjectId = Guid.NewGuid().ToString();

                var serialized = Serializer.SerializeToXml(request);

                Assert.NotNull(serialized);

                var expected = $@"
<tsRequest>
    <project 
        parentProjectId=""{request.Project.ParentProjectId}"" 
        name=""{request.Project.Name}"" 
        description=""{request.Project.Description}"" 
        contentPermissions=""{request.Project.ContentPermissions}"" />
</tsRequest>";

                AssertXmlEqual(expected, serialized);
            }
        }

        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes_with_values()
            {
                var parentProjectId = Create<Guid>();
                var name = Create<string>();
                var description = Create<string>();
                var contentPermissions = Create<string>();

                var request = new CreateProjectRequest(parentProjectId, name, description, contentPermissions);

                Assert.NotNull(request.Project);

                Assert.Equal(parentProjectId, Guid.Parse(request.Project.ParentProjectId!));
                Assert.Equal(name, request.Project.Name);
                Assert.Equal(description, request.Project.Description);
                Assert.Equal(contentPermissions, request.Project.ContentPermissions);
            }

            [Fact]
            public void Handles_null_optional_values()
            {
                var name = Create<string>();

                var request = new CreateProjectRequest(null, name, null, null);

                Assert.NotNull(request.Project);

                Assert.Null(request.Project.ParentProjectId);
                Assert.Equal(name, request.Project.Name);
                Assert.Null(request.Project.Description);
                Assert.Null(request.Project.ContentPermissions);
            }

            [Fact]
            public void Initializes_with_options()
            {
                var options = Create<ICreateProjectOptions>();

                var request = new CreateProjectRequest(options);

                Assert.NotNull(request.Project);

                Assert.Equal(options.ParentProject?.Id, Guid.Parse(request.Project.ParentProjectId!));
                Assert.Equal(options.Name, request.Project.Name);
                Assert.Equal(options.Description, request.Project.Description);
                Assert.Equal(options.ContentPermissions, request.Project.ContentPermissions);
            }
        }
    }
}