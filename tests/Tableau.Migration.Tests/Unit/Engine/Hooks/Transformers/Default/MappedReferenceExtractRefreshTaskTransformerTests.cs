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

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class MappedReferenceExtractRefreshTaskTransformerTests
    {
        public abstract class MappedReferenceExtractRefreshTaskTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockDestinationContentReferenceFinderFactory = new();
            protected readonly Mock<ILogger<MappedReferenceExtractRefreshTaskTransformer>> MockLogger = new();
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
            protected readonly Mock<IDestinationContentReferenceFinder<IWorkbook>> MockWorkbookContentFinder = new();
            protected readonly Mock<IDestinationContentReferenceFinder<IDataSource>> MockDatasourceContentFinder = new();

            protected readonly MappedReferenceExtractRefreshTaskTransformer Transformer;

            public MappedReferenceExtractRefreshTaskTransformerTest()
            {
                MockDestinationContentReferenceFinderFactory.Setup(p => p.ForDestinationContentType<IWorkbook>()).Returns(MockWorkbookContentFinder.Object);
                MockDestinationContentReferenceFinderFactory.Setup(p => p.ForDestinationContentType<IDataSource>()).Returns(MockDatasourceContentFinder.Object);

                Transformer = new(MockDestinationContentReferenceFinderFactory.Object, MockSharedResourcesLocalizer.Object, MockLogger.Object);
            }
        }

        public class ExecuteAsync : MappedReferenceExtractRefreshTaskTransformerTest
        {
            [Fact]
            public async Task Returns_the_same_object()
            {
                var extractRefreshTask = Create<ICloudExtractRefreshTask>();

                var result = await Transformer.TransformAsync(extractRefreshTask, Cancel);

                Assert.NotNull(result);
                Assert.Same(extractRefreshTask, result);
                MockLogger.VerifyWarnings(Times.Once);
            }

            [Fact]
            public async Task Returns_destination_workbook_when_found()
            {
                var extractRefreshTask = Create<ICloudExtractRefreshTask>();
                var sourceWorkbook = Create<IContentReference>();
                var destinationWorkbook = Create<IContentReference>();
                extractRefreshTask.ContentType = ExtractRefreshContentType.Workbook;
                extractRefreshTask.Content = sourceWorkbook;

                MockWorkbookContentFinder.Setup(f => f.FindBySourceIdAsync(sourceWorkbook.Id, Cancel)).ReturnsAsync(destinationWorkbook);

                var result = await Transformer.TransformAsync(extractRefreshTask, Cancel);

                Assert.NotNull(result);
                Assert.Same(extractRefreshTask, result);
                MockLogger.VerifyWarnings(Times.Never);
                Assert.Same(destinationWorkbook, result.Content);
            }

            [Fact]
            public async Task Returns_destination_datasource_when_found()
            {
                var extractRefreshTask = Create<ICloudExtractRefreshTask>();
                var sourceDataSource = Create<IContentReference>();
                var destinationDataSource = Create<IContentReference>();
                extractRefreshTask.ContentType = ExtractRefreshContentType.DataSource;
                extractRefreshTask.Content = sourceDataSource;

                MockDatasourceContentFinder.Setup(f => f.FindBySourceIdAsync(sourceDataSource.Id, Cancel)).ReturnsAsync(destinationDataSource);

                var result = await Transformer.TransformAsync(extractRefreshTask, Cancel);

                Assert.NotNull(result);
                Assert.Same(extractRefreshTask, result);
                MockLogger.VerifyWarnings(Times.Never);
                Assert.Same(destinationDataSource, result.Content);
            }
        }
    }
}
