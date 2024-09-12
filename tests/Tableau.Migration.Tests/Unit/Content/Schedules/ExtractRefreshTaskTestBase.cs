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

using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Tests.Unit.Content.Schedules
{
    public abstract class ExtractRefreshTaskTestBase : ScheduleTestBase
    {
        protected readonly Mock<IContentReferenceFinderFactory> MockFinderFactory = new() { CallBase = true };

        protected readonly Mock<IContentReferenceFinder<IDataSource>> MockDataSourceFinder = new();

        protected readonly Mock<IContentCache<IServerSchedule>> MockScheduleCache = new();

        protected readonly Mock<IContentReferenceFinder<IServerSchedule>> MockScheduleFinder = new();

        protected readonly Mock<IContentReferenceFinder<IWorkbook>> MockWorkbookFinder = new();

        protected ILogger Logger { get; }

        protected ISharedResourcesLocalizer Localizer { get; }

        protected ExtractRefreshTestCaches ExtractRefreshTestCaches { get; }

        protected ExtractRefreshTaskTestBase()
        {
            Logger = Create<ILogger>();
            Localizer = Create<ISharedResourcesLocalizer>();

            MockFinderFactory.Setup(x => x.ForContentType<IDataSource>()).Returns(MockDataSourceFinder.Object);
            MockFinderFactory.Setup(x => x.ForContentType<IWorkbook>()).Returns(MockWorkbookFinder.Object);

            ExtractRefreshTestCaches = new(AutoFixture, MockDataSourceFinder, MockWorkbookFinder, MockScheduleFinder, MockScheduleCache);
        }
    }
}
