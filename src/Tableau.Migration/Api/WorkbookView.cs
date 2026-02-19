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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Implementation of a workbook view.
    /// </summary>
    internal sealed class WorkbookView : IWorkbookView
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string? ContentUrl { get; }

        /// <inheritdoc />
        public string? ViewUrlName { get; }

        /// <inheritdoc />
        public string? CreatedAt { get; }

        /// <inheritdoc />
        public string? UpdatedAt { get; }

        /// <inheritdoc />
        public IList<ITag> Tags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbookView"/> class.
        /// </summary>
        /// <param name="viewResponse">The view response to initialize from.</param>
        public WorkbookView(IWorkbookViewType viewResponse)
        {
            Id = viewResponse.Id;
            Name = Guard.AgainstNullEmptyOrWhiteSpace(viewResponse.Name, () => viewResponse.Name);
            ContentUrl = viewResponse.ContentUrl;
            ViewUrlName = viewResponse.ViewUrlName;
            CreatedAt = viewResponse.CreatedAt;
            UpdatedAt = viewResponse.UpdatedAt;
            Tags = viewResponse.Tags.ToTagList(t => new Tag(t));
        }
    }
}