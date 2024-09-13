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
using System.Threading;
using AutoFixture;
using Moq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Tests.Unit.Content.Schedules
{
    public sealed class ExtractRefreshTestCaches
    {
        private readonly IFixture _fixture;

        private readonly Mock<IContentCache<IServerSchedule>> _mockServerScheduleCache;
        private readonly Mock<IContentReferenceFinder<IDataSource>> _mockDataSourceFinder;
        private readonly Mock<IContentReferenceFinder<IServerSchedule>> _mockServerScheduleFinder;
        private readonly Mock<IContentReferenceFinder<IWorkbook>> _mockWorkbookFinder;

        public ExtractRefreshTestCaches(IFixture fixture, 
            Mock<IContentReferenceFinder<IDataSource>> mockDataSourceFinder, Mock<IContentReferenceFinder<IWorkbook>> mockWorkbookFinder,
            Mock<IContentReferenceFinder<IServerSchedule>> mockServerScheduleFinder, Mock<IContentCache<IServerSchedule>> mockServerScheduleCache)
        {
            _fixture = fixture;
            _mockDataSourceFinder = mockDataSourceFinder;
            _mockServerScheduleCache = mockServerScheduleCache;
            _mockServerScheduleFinder = mockServerScheduleFinder;
            _mockWorkbookFinder = mockWorkbookFinder;

            // Set default finder behavior to return null on missing items.
            _mockDataSourceFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((IContentReference?)null);
            _mockWorkbookFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((IContentReference?)null);
        }

        public void SetupExtractRefreshContentFinder(IExtractRefreshType extractRefresh)
        {
            var contentType = extractRefresh.GetContentType();
            var contentId = extractRefresh.GetContentId();

            var mockReference = _fixture.Create<Mock<IContentReference>>();
            mockReference.SetupGet(r => r.Id).Returns(contentId);

            var contentReference = mockReference.Object;

            switch (contentType)
            {
                case ExtractRefreshContentType.DataSource:
                    SetupContentFinder(_mockDataSourceFinder);
                    break;

                case ExtractRefreshContentType.Workbook:
                    SetupContentFinder(_mockWorkbookFinder);
                    break;

                default:
                    throw new NotSupportedException($"Content type {contentType} is not supported.");
            }

            if (extractRefresh is IServerExtractRefreshType serverExtractRefresh)
            {
                Guard.AgainstNull(serverExtractRefresh.Schedule, () => serverExtractRefresh.Schedule);

                var scheduleId = serverExtractRefresh.Schedule.Id;

                var mockSchedule = _fixture.Create<Mock<IServerSchedule>>();
                mockSchedule.SetupGet(s => s.Id).Returns(scheduleId);

                var cachedSchedule = mockSchedule.Object;
                var scheduleReference = cachedSchedule.ToStub();

                _mockServerScheduleFinder.Setup(f => f.FindByIdAsync(scheduleId, It.IsAny<CancellationToken>())).ReturnsAsync(scheduleReference);

                _mockServerScheduleCache.Setup(f => f.ForIdAsync(scheduleId, It.IsAny<CancellationToken>())).ReturnsAsync(cachedSchedule);
            }

            void SetupContentFinder<T>(Mock<IContentReferenceFinder<T>> mockFinder)
                where T : IContentReference
                => mockFinder.Setup(f => f.FindByIdAsync(contentId, It.IsAny<CancellationToken>())).ReturnsAsync(contentReference);
        }

        public void SetupExtractRefreshContentFinder(IEnumerable<IExtractRefreshType> extractRefreshes)
        {
            foreach (var extractRefresh in extractRefreshes)
                SetupExtractRefreshContentFinder(extractRefresh);
        }

    }
}
