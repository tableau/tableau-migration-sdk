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
    public static class IWorkbookExtensions
    {
        public static void Assert<TWorkbook>(
            this TWorkbook actual,
            IWorkbook expected,
            Action<TWorkbook>? extra = null)
            where TWorkbook : IWorkbook =>
                actual.Assert(
                    expected.Id,
                    expected.Name,
                    expected.ContentUrl,
                    expected.Description,
                    expected.CreatedAt,
                    expected.UpdatedAt,
                    expected.EncryptExtracts,
                    expected.ShowTabs,
                    expected.Size,
                    expected.WebpageUrl,
                    expected.Tags,
                    ((IContainerContent)expected).Container,
                    expected.Owner,
                    extra
                );

        public static void Assert<TWorkbook>(
            this TWorkbook actual,
            IWorkbook expected,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TWorkbook>? extra = null)
            where TWorkbook : IWorkbook =>
                actual.Assert(
                    expected.Id,
                    expectedProject.Name,
                    expectedOwner.ContentUrl,
                    expected.Description,
                    expected.CreatedAt,
                    expected.UpdatedAt,
                    expected.EncryptExtracts,
                    expected.ShowTabs,
                    expected.Size,
                    expected.WebpageUrl,
                    expected.Tags.Select(t => new Tag(t)),
                    expectedProject,
                    expectedOwner,
                    extra
                );

        public static void Assert<TWorkbook>(
            this TWorkbook actual,
            IWorkbookType expected,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TWorkbook>? extra = null)
            where TWorkbook : IWorkbook
        {
            NotNull(actual);
            Equal(expected.Id, actual.Id);
            Equal(expected.Name, actual.Name);
            Equal(expected.ContentUrl, actual.ContentUrl);

            Equal(expected.Description, actual.Description);
            Equal(expected.CreatedAt, actual.CreatedAt);
            Equal(expected.UpdatedAt, actual.UpdatedAt);

            Equal(expected.EncryptExtracts, actual.EncryptExtracts);
            Equal(expected.ShowTabs, actual.ShowTabs);
            Equal(expected.Size, actual.Size);

            Equal(expected.WebpageUrl, actual.WebpageUrl);

            var actualProject = ((IContainerContent)actual).Container;
            NotNull(actualProject);

            Same(expectedProject, actualProject);
            Equal(expectedProject.Location.Append(actual.Name), actual.Location);

            Same(expectedOwner, actual.Owner);

            expected.Tags.AssertEqual(actual.Tags);

            extra?.Invoke(actual);
        }

        public static void Assert<TWorkbook>(
            this TWorkbook actual,
            Guid expectedId,
            string? expectedName,
            string? expectedContentUrl,
            string? expectedDescription,
            string? expectedCreatedAt,
            string? expectedUpdatedAt,
            bool expectedEncryptExtracts,
            bool expectedShowTabs,
            long expectedSize,
            string? expectedWebpageUrl,
            IEnumerable<ITag> expectedTags,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TWorkbook>? extra = null)
            where TWorkbook : IWorkbook
        {
            NotNull(actual);
            Equal(expectedId, actual.Id);
            Equal(expectedName, actual.Name);
            Equal(expectedContentUrl, actual.ContentUrl);

            Equal(expectedDescription, actual.Description);
            Equal(expectedCreatedAt, actual.CreatedAt);
            Equal(expectedUpdatedAt, actual.UpdatedAt);

            Equal(expectedEncryptExtracts, actual.EncryptExtracts);
            Equal(expectedShowTabs, actual.ShowTabs);
            Equal(expectedSize, actual.Size);

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
