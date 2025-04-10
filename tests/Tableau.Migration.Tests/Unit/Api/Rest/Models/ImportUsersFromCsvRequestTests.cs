//
//  Copyright (c) 2025, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
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
            var request = new ImportUsersFromCsvRequest(UserAuthenticationType.ForAuthenticationType(AuthenticationTypes.Saml));

            Assert.NotEmpty(request.Users);

            var serialized = Serializer.SerializeToXml(request);

            Assert.NotNull(serialized);

            var expected = $@"<tsRequest><user authSetting=""SAML"" /></tsRequest>";

            AssertXmlEqual(expected, serialized);
        }

        [Fact]
        public void SerializesDefaultIdpConfigurationId()
        {
            var id = Guid.NewGuid();
            var request = new ImportUsersFromCsvRequest(UserAuthenticationType.ForConfigurationId(id));

            Assert.NotEmpty(request.Users);

            var serialized = Serializer.SerializeToXml(request);

            Assert.NotNull(serialized);

            var expected = $@"<tsRequest><user idpConfigurationId=""{id}"" /></tsRequest>";

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

            var expected = $@"<tsRequest>{string.Join("", users.Select(u => $@"<user name=""{u.Name}"" authSetting=""{u.AuthSetting}"" idpConfigurationId=""{u.IdpConfigurationId}"" />"))}</tsRequest>";

            AssertXmlEqual(expected, serialized);
        }
    }
}
