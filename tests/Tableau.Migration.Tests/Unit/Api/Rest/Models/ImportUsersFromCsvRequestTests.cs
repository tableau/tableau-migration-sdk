using System.Linq;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Types;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class ImportUsersFromCsvRequestTests : SerializationTestBase
    {
        [Fact]
        public void SerializesDefaultUser()
        {
            var request = new ImportUsersFromCsvRequest();

            Assert.NotEmpty(request.Users);

            var serialized = Serializer.SerializeToXml(request);

            Assert.NotNull(serialized);

            var expected = $@"<tsRequest><user /></tsRequest>";

            AssertXmlEqual(expected, serialized);
        }

        [Fact]
        public void SerializesDefaultAuthType()
        {
            var request = new ImportUsersFromCsvRequest(AuthenticationTypes.Saml);

            Assert.NotEmpty(request.Users);

            var serialized = Serializer.SerializeToXml(request);

            Assert.NotNull(serialized);

            var expected = $@"<tsRequest><user authSetting=""SAML"" /></tsRequest>";

            AssertXmlEqual(expected, serialized);
        }

        [Fact]
        public void SerializesMultipleUsers()
        {
            var users = CreateMany<ImportUsersFromCsvRequest.UserType>();
            var request = new ImportUsersFromCsvRequest(users);

            Assert.NotEmpty(request.Users);

            var serialized = Serializer.SerializeToXml(request);

            Assert.NotNull(serialized);

            var expected = $@"<tsRequest>{string.Join("", users.Select(u => $@"<user name=""{u.Name}"" authSetting=""{u.AuthSetting}"" />"))}</tsRequest>";

            AssertXmlEqual(expected, serialized);
        }
    }
}
