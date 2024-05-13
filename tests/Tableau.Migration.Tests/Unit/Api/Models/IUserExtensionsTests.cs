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

using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class IUserExtensionsTests
    {
        public class AppendCsvLine
        {
            #region - Test Data -

            public class CsvTestData
            {
                public CsvTestData(IUser user, string expectedCsvLine)
                {
                    User = user;
                    ExpectedCsvLine = expectedCsvLine;
                }

                public IUser User { get; }

                public string ExpectedCsvLine { get; }
            }

            public static IUser TestUser(string name, string fullName, string email, string siteRole)
            {
                var mockUser = new Mock<IUser> { CallBase = true };
                mockUser.SetupGet(x => x.Name).Returns(name);
                mockUser.SetupGet(x => x.FullName).Returns(fullName);
                mockUser.SetupGet(x => x.Email).Returns(email);
                mockUser.SetupGet(x => x.SiteRole).Returns(siteRole);

                return mockUser.Object;
            }

            public static IEnumerable<object[]> GetCsvTestData()
            {
                yield return new object[]
                {
                   new CsvTestData(
                       user: TestUser("henryw", "Henry Wilson", "henryw@example.com", "Creator"),
                       expectedCsvLine: $"henryw,,Henry Wilson,Creator,None,True,henryw@example.com{Environment.NewLine}")
                };
                yield return new object[]
                {
                    new CsvTestData(
                       user: TestUser("henryw", "Henry Wilson", "henryw@example.com", SiteRoles.Explorer),
                       expectedCsvLine: $"henryw,,Henry Wilson,Explorer,None,False,henryw@example.com{Environment.NewLine}")
                };
                yield return new object[]
                {
                    new CsvTestData(
                       user: TestUser("henryw", "William O'Connor", "henryw@example.com", SiteRoles.Explorer),
                       expectedCsvLine: $"henryw,,William O'Connor,Explorer,None,False,henryw@example.com{Environment.NewLine}")
                };
            }

            #endregion

            [Theory]
            [MemberData(nameof(GetCsvTestData))]
            public void BuildsCsvString(CsvTestData testData)
            {
                var sb = new StringBuilder();
                testData.User.AppendCsvLine(sb);
                Assert.Equal(testData.ExpectedCsvLine, sb.ToString());
            }
        }
    }
}
