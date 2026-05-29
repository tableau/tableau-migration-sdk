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
using System.Collections.Generic;
using Moq;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default.Cascade
{
    public abstract class CascadingFilterTestBase : AutoFixtureTestBase
    {
        protected readonly Mock<IMigrationManifestEntryCollectionEditor> MockManifestEntryCollection;
        protected readonly Mock<IMigrationManifestContentTypePartitionEditor> MockManifestPartition;

        protected Dictionary<ContentLocation, bool> CascadeSkipEntries = new();

        public CascadingFilterTestBase()
        {
            MockManifestEntryCollection = Freeze<Mock<IMigrationManifestEntryCollectionEditor>>();
            MockManifestPartition = Freeze<Mock<IMigrationManifestContentTypePartitionEditor>>();

            bool BuildCascadeEntry(ContentLocation loc, out IMigrationManifestEntryEditor entry)
            {
                if (CascadeSkipEntries.TryGetValue(loc, out var cascade))
                {
                    var mockEntry = Create<Mock<IMigrationManifestEntryEditor>>();
                    mockEntry.SetupGet(x => x.CascadeSkip).Returns(cascade);

                    entry = mockEntry.Object;
                    return true;
                }

                entry = null!;
                return false;
            }

            MockManifestPartition.Setup(x => x.BySourceLocation.TryGetValue(It.IsAny<ContentLocation>(), out It.Ref<IMigrationManifestEntryEditor>.IsAny!))
                .Returns(BuildCascadeEntry);
        }

        protected void AssertContentReferenceTypeSearched<TReference>()
            => AssertContentReferenceTypeSearched(typeof(TReference));

        protected void AssertContentReferenceTypeSearched(Type contentType)
        {
            MockManifestEntryCollection.Verify(x => x.GetPartition(contentType), Times.AtLeastOnce);
        }
    }
}
