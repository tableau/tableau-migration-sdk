﻿//
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
using System.Threading;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class ContentItemPreparerTestBase<TPrepare, TPublish> : AutoFixtureTestBase
        where TPrepare : class
        where TPublish : class
    {
        protected readonly Mock<IMigrationPipeline> MockPipeline;
        protected readonly Mock<IContentTransformerRunner> MockTransformerRunner;
        protected readonly Mock<IMigrationManifestEntryEditor> MockManifestEntry;
        protected readonly Mock<IDestinationContentReferenceFinder<IProject>> MockProjectFinder;
        protected readonly Mock<IContentFileStore> MockFileStore;
        protected readonly ContentMigrationItem<TPrepare> Item;

        protected ContentLocation MappedLocation { get; set; }

        public ContentItemPreparerTestBase()
        {
            MockPipeline = Freeze<Mock<IMigrationPipeline>>();
            MockPipeline.Setup(x => x.GetItemConverter<It.IsAnyType, It.IsAnyType>())
                .Returns(new InvocationFunc(invocation =>
                {
                    var genericArgs = invocation.Method.GetGenericArguments();
                    return Activator.CreateInstance(typeof(DirectContentItemConverter<,>).MakeGenericType(genericArgs))!;
                }));

            MockTransformerRunner = Freeze<Mock<IContentTransformerRunner>>();
            MockTransformerRunner.Setup(x => x.ExecuteAsync(It.IsAny<TPublish>(), Cancel))
                .ReturnsAsync((TPublish item, CancellationToken cancel) => item);

            MappedLocation = Create<ContentLocation>();

            MockManifestEntry = Freeze<Mock<IMigrationManifestEntryEditor>>();
            MockManifestEntry.SetupGet(x => x.MappedLocation).Returns(() => MappedLocation);

            MockProjectFinder = Freeze<Mock<IDestinationContentReferenceFinder<IProject>>>();
            MockProjectFinder.Setup(x => x.FindBySourceLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                    .ReturnsAsync((IContentReference?)null);

            var mockDestinationFinderFactory = Freeze<Mock<IDestinationContentReferenceFinderFactory>>();
            mockDestinationFinderFactory.Setup(x => x.ForDestinationContentType<IProject>())
                .Returns(MockProjectFinder.Object);

            MockFileStore = Freeze<Mock<IContentFileStore>>();

            Item = Create<ContentMigrationItem<TPrepare>>();
        }
    }

    public class ContentItemPreparerTestBase<TPrepare> : ContentItemPreparerTestBase<TPrepare, TPrepare>
        where TPrepare : class
    { }
}
