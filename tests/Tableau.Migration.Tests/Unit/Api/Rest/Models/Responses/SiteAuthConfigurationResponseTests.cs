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

using System.Text;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public sealed class SiteAuthConfigurationResponseTests
    {
        public sealed class Deserialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedResponse = Create<SiteAuthConfigurationsResponse>();

                var itemsXml = new StringBuilder();
                foreach(var item in expectedResponse.Items)
                {
                    itemsXml.AppendLine($@"<siteAuthConfiguration 
                        authSetting=""{item.AuthSetting}""
                        knownProviderAlias=""{item.KnownProviderAlias}""
                        idpConfigurationName=""{item.IdpConfigurationName}""
                        idpConfigurationId=""{item.IdpConfigurationId}""
                        enabled=""{item.Enabled.ToString().ToLower()}"" />");
                }

                var xml = $@"
<tsResponse>
    <siteAuthConfigurations>
        {itemsXml}
    </siteAuthConfigurations>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<SiteAuthConfigurationsResponse>(xml);
                Assert.NotNull(deserialized);
                Assert.Equal(expectedResponse.Items, deserialized.Items, 
                (SiteAuthConfigurationsResponse.SiteAuthConfigurationType a, SiteAuthConfigurationsResponse.SiteAuthConfigurationType b) =>
                {
                    return a.AuthSetting == b.AuthSetting &&
                    a.IdpConfigurationId == b.IdpConfigurationId &&
                    a.IdpConfigurationName == b.IdpConfigurationName &&
                    a.KnownProviderAlias == b.KnownProviderAlias &&
                    a.Enabled == b.Enabled;
                });
            }
        }
    }
}
