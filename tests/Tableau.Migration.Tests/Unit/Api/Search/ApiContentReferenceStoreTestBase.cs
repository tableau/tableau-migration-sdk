//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Config;

namespace Tableau.Migration.Tests.Unit.Api.Search
{
    public abstract class ApiContentReferenceStoreTestBase<TContent> : AutoFixtureTestBase
        where TContent : class, IContentReference
    {
        protected readonly Mock<IConfigReader> MockConfigReader;
        protected readonly Mock<ISitesApiClient> MockSitesApiClient;
        protected readonly Mock<IPagedListApiClient<TContent>> MockListApiClient;
        protected readonly Mock<IReadApiClient<TContent>> MockReadApiClient;
        protected readonly Mock<INameSearchApiClient<TContent>> MockNameSearchApiClient;
        protected readonly Mock<IContentUrlSearchApiClient<TContent>> MockContentUrlSearchApiClient;

        protected ContentTypesOptions ContentTypesOptions { get; set; } = new ContentTypesOptions();

        protected List<TContent> EndpointContent { get; set; }

        public ApiContentReferenceStoreTestBase()
        {
            EndpointContent = CreateMany<TContent>().ToList();
            ContentTypesOptions.BatchSize = EndpointContent.Count / 2;

            MockListApiClient = Freeze<Mock<IPagedListApiClient<TContent>>>();
            MockListApiClient.Setup(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Result<IImmutableList<TContent>>.Succeeded(EndpointContent.ToImmutableList()));

            MockReadApiClient = Freeze<Mock<IReadApiClient<TContent>>>();
            MockReadApiClient.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken _) =>
                {
                    var item = EndpointContent.SingleOrDefault(x => x.Id == id);
                    if (item is null)
                        return Result<TContent>.Failed(new Exception("not found"));
                    else
                        return Result<TContent>.Succeeded(item);
                });

            MockNameSearchApiClient = Freeze<Mock<INameSearchApiClient<TContent>>>();
            MockNameSearchApiClient.Setup(x => x.SearchByNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string name, int _, CancellationToken _) =>
                    Result<IImmutableList<TContent>>.Succeeded(EndpointContent.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)).ToImmutableList())
                );

            MockContentUrlSearchApiClient = Freeze<Mock<IContentUrlSearchApiClient<TContent>>>();
            MockContentUrlSearchApiClient.Setup(x => x.SearchByContentUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string url, CancellationToken _) =>
                    Result<IImmutableList<TContent>>.Succeeded(EndpointContent.Where(x => string.Equals(x.ContentUrl, url, StringComparison.Ordinal)).ToImmutableList())
                );

            MockSitesApiClient = Freeze<Mock<ISitesApiClient>>();
            MockSitesApiClient.Setup(x => x.GetListApiClient<TContent>())
                .Returns(MockListApiClient.Object);
            MockSitesApiClient.Setup(x => x.GetReadApiClient<TContent>())
                .Returns(MockReadApiClient.Object);
            MockSitesApiClient.Setup(x => x.GetNameSearchApiClient<TContent>())
                .Returns(MockNameSearchApiClient.Object);
            MockSitesApiClient.Setup(x => x.GetContentUrlSearchApiClient<TContent>())
                .Returns(MockContentUrlSearchApiClient.Object);

            MockConfigReader = Freeze<Mock<IConfigReader>>();
            MockConfigReader.Setup(x => x.Get<TestContentType>())
                .Returns(() => ContentTypesOptions);
        }
    }
}
