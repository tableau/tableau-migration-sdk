//
//  Copyright (c) 2024, Salesforce, Inc.
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

using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public class CustomViewAsUsersDefaultViewResponseTest
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes_custom_views_as_Default_user_view()
            {
                var expectedResponse = Create<CustomViewAsUsersDefaultViewResponse>();
                var expectedCustomViewAsUserDefaultViewType = expectedResponse.Items.First();


                var xml = @$"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api http://tableau.com/api/ts-api-exp.xsd"">
    <customViewAsUserDefaultResults>
        <customViewAsUserDefaultViewResult success=""{expectedCustomViewAsUserDefaultViewType.Success.ToString().ToLower()}"">
            <user id=""{expectedCustomViewAsUserDefaultViewType.User?.Id}""/>
        </customViewAsUserDefaultViewResult>
    </customViewAsUserDefaultResults>
</tsResponse>
";
                var deserialized = Serializer.DeserializeFromXml<CustomViewAsUsersDefaultViewResponse>(xml);
                Assert.NotNull(deserialized);
                Assert.Single(deserialized.Items);

                var actualCustomViewAsUserDefaultViewType = deserialized.Items[0];
                Assert.NotNull(actualCustomViewAsUserDefaultViewType);

                Assert.Equal(expectedCustomViewAsUserDefaultViewType.Success, actualCustomViewAsUserDefaultViewType.Success);
                Assert.Equal(expectedCustomViewAsUserDefaultViewType.User?.Id, actualCustomViewAsUserDefaultViewType.User?.Id);
            }
        }
    }
}
