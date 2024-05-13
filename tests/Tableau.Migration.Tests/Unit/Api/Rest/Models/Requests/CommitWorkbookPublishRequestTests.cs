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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class CommitWorkbookPublishRequestTests
    {
        public abstract class CommitWorkbookPublishRequestTest : AutoFixtureTestBase
        { }

        public class Ctor : CommitWorkbookPublishRequestTest
        {
            [Fact]
            public void Initializes()
            {
                var options = Create<IPublishWorkbookOptions>();

                var request = new CommitWorkbookPublishRequest(options);

                Assert.NotNull(request.Workbook);

                Assert.Equal(options.Name, request.Workbook.Name);
                Assert.Equal(options.Description, request.Workbook.Description);
                Assert.Equal(options.ShowTabs, request.Workbook.ShowTabs);
                Assert.Equal(options.ThumbnailsUserId, request.Workbook.ThumbnailsUserId);
                Assert.Equal(options.EncryptExtracts, request.Workbook.EncryptExtracts);

                Assert.NotNull(request.Workbook.Project);

                Assert.Equal(options.ProjectId, request.Workbook.Project.Id);

                Assert.All(options.HiddenViewNames, v => Assert.Single(request.Workbook.Views.Where(wbv => wbv.Name == v && wbv.Hidden)));
            }
        }
    }
}
