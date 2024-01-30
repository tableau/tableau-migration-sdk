// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal class Workbook : ContainerContentBase, IWorkbook
    {
        public Workbook(IWorkbookType response, IContentReference project, IContentReference owner)
            : base(project)
        {
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            ContentUrl = Guard.AgainstNullEmptyOrWhiteSpace(response.ContentUrl, () => response.ContentUrl);

            ShowTabs = response.ShowTabs;
            Size = response.Size;
            WebpageUrl = response.WebpageUrl;
            EncryptExtracts = response.EncryptExtracts;

            Description = response.Description ?? string.Empty;
            CreatedAt = response.CreatedAt ?? string.Empty;
            UpdatedAt = response.UpdatedAt ?? string.Empty;
            WebpageUrl = response.WebpageUrl ?? string.Empty;

            Owner = owner;
            Tags = response.Tags.ToTagList(t => new Tag(t));

            Location = project.Location.Append(Name);
        }

        /// <inheritdoc />
        public bool ShowTabs { get; set; }

        /// <inheritdoc />
        public long Size { get; }

        /// <inheritdoc />
        public string? WebpageUrl { get; }

        /// <inheritdoc />
        public string CreatedAt { get; }

        /// <inheritdoc />
        public string? UpdatedAt { get; }

        /// <inheritdoc />
        public bool EncryptExtracts { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IList<ITag> Tags { get; set; }

        /// <inheritdoc />
        public IContentReference Owner { get; set; }
    }
}
