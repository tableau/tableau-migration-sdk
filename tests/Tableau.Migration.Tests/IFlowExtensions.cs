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
using System.Linq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using static Xunit.Assert;

namespace Tableau.Migration.Tests.Unit.Content
{
    public static class IFlowExtensions
    {
        public static void Assert<TFlow>(
            this TFlow actual,
            IFlow expected,
            Action<TFlow>? extra = null)
            where TFlow : IFlow =>
                actual.Assert(
                    expected.Id,
                    expected.Name,
                    expected.Description,
                    expected.WebpageUrl,
                    expected.FileType,
                    expected.CreatedAt,
                    expected.UpdatedAt,
                    expected.Tags,
                    ((IContainerContent)expected).Container,
                    expected.Owner,
                    extra
                );

        public static void Assert<TFlow>(
            this TFlow actual,
            IFlowType expected,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TFlow>? extra = null)
            where TFlow : IFlow =>
                actual.Assert(
                    expected.Id,
                    expected.Name,
                    expected.Description,
                    expected.WebpageUrl,
                    expected.FileType,
                    expected.CreatedAt,
                    expected.UpdatedAt,
                    expected.Tags.Select(t => new Tag(t)),
                    expectedProject,
                    expectedOwner,
                    extra
                );

        public static void Assert<TFlow>(
            this TFlow actual,
            IFlowDetailsType expected,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TFlow>? extra = null)
            where TFlow : IFlow =>
                actual.Assert(
                    expected.Id,
                    expected.Name,
                    expected.Description,
                    expected.WebpageUrl,
                    expected.FileType,
                    expected.CreatedAt,
                    expected.UpdatedAt,
                    expected.Tags.Select(t => new Tag(t)),
                    expectedProject,
                    expectedOwner,
                    extra
                );

        public static void Assert<TFlow>(
            this TFlow actual,
            Guid expectedId,
            string? expectedName,
            string? expectedDescription,
            string? expectedWebpageUrl,
            string? expectedFileType,
            string? expectedCreatedAt,
            string? expectedUpdatedAt,
            IEnumerable<ITag> expectedTags,
            IContentReference expectedProject,
            IContentReference expectedOwner,
            Action<TFlow>? extra = null)
            where TFlow : IFlow
        {
            Equal(expectedId, actual.Id);
            Equal(expectedName, actual.Name);
            Equal(expectedDescription, actual.Description);
            Equal(expectedWebpageUrl, actual.WebpageUrl);
            Equal(expectedFileType, actual.FileType);
            Equal(expectedCreatedAt, actual.CreatedAt);
            Equal(expectedUpdatedAt, actual.UpdatedAt);

            Same(expectedProject, ((IContainerContent)actual).Container);
            Same(expectedOwner, actual.Owner);

            Equal(expectedTags.Count(), actual.Tags.Count);
            All(actual.Tags, tag =>
            {
                Contains(tag, expectedTags, TagLabelComparer.Instance);
            });

            extra?.Invoke(actual);
        }
    }
}

