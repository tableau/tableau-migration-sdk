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
using System.Linq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

using static Xunit.Assert;

namespace Tableau.Migration.Tests.Unit.Content
{
    public static class IDataSourceExtensions
    {
        public static void Assert<TDataSource>(
            this TDataSource actual,
            IDataSource expected,
            Action<TDataSource>? extra = null)
            where TDataSource : IDataSource =>
                actual.Assert(
                    expected.Id,
                    expected.Name,
                    expected.ContentUrl,
                    expected.Description,
                    expected.CreatedAt,
                    expected.UpdatedAt,
                    expected.EncryptExtracts,
                    expected.HasExtracts,
                    expected.IsCertified,
                    expected.UseRemoteQueryAgent,
                    expected.WebpageUrl,
                    expected.Tags,
                    ((IContainerContent)expected).Container,
                    expected.Owner,
                    extra
                );

        public static void Assert<TDataSource>(
            this TDataSource actual,
            IDataSourceType expected,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TDataSource>? extra = null)
            where TDataSource : IDataSource =>
                actual.Assert(
                    expected.Id,
                    expected.Name,
                    expected.ContentUrl,
                    expected.Description,
                    expected.CreatedAt,
                    expected.UpdatedAt,
                    expected.EncryptExtracts,
                    expected.HasExtracts,
                    expected.IsCertified,
                    expected.UseRemoteQueryAgent,
                    expected.WebpageUrl,
                    expected.Tags.Select(t => new Tag(t)),
                    expectedProject,
                    expectedOwner,
                    extra
                );

        public static void Assert<TDataSource>(
            this TDataSource actual,
            Guid expectedId,
            string? expectedName,
            string? expectedContentUrl,
            string? expectedDescription,
            string? expectedCreatedAt,
            string? expectedUpdatedAt,
            bool expectedEncryptExtracts,
            bool expectedHasExtracts,
            bool expectedIsCertified,
            bool expectedUseRemoteQueryAgent,
            string? expectedWebpageUrl,
            IEnumerable<ITag> expectedTags,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TDataSource>? extra = null)
            where TDataSource : IDataSource
        {
            NotNull(actual);
            Equal(expectedId, actual.Id);
            Equal(expectedName, actual.Name);
            Equal(expectedContentUrl, actual.ContentUrl);

            Equal(expectedDescription, actual.Description);
            Equal(expectedCreatedAt, actual.CreatedAt);
            Equal(expectedUpdatedAt, actual.UpdatedAt);

            Equal(expectedEncryptExtracts, actual.EncryptExtracts);
            Equal(expectedHasExtracts, actual.HasExtracts);
            Equal(expectedIsCertified, actual.IsCertified);
            Equal(expectedUseRemoteQueryAgent, actual.UseRemoteQueryAgent);

            Equal(expectedWebpageUrl, actual.WebpageUrl);

            var actualProject = ((IContainerContent)actual).Container;
            NotNull(actualProject);

            Same(expectedProject, actualProject);
            Equal(expectedProject.Location.Append(actual.Name), actual.Location);

            Same(expectedOwner, actual.Owner);

            expectedTags.AssertEqual(actual.Tags);

            extra?.Invoke(actual);
        }
    }
}
