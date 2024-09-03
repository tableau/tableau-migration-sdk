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
using System.IO;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{

    /// <inheritdoc/>    
    public class PublishCustomViewOptions : PublishContentWithFileOptions, IPublishCustomViewOptions
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public bool Shared { get; set; }

        /// <inheritdoc/>
        public Guid WorkbookId { get; set; }

        /// <inheritdoc/>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Creates a new <see cref="PublishCustomViewOptions"/> instance.
        /// </summary>
        /// <param name="customView">The publishable custom view information.</param>
        /// <param name="file">The custom view file as a <see cref="Stream"/>.</param>
        /// <param name="fileType">The type of custom view file. Defaults to <see cref="CustomViewFileTypes.Json"/>.</param> 
        public PublishCustomViewOptions(
            IPublishableCustomView customView,
            Stream file,
            string fileType = CustomViewFileTypes.Json)
            : base(
                  file,
                  customView.File.OriginalFileName,
                  fileType)
        {
            Name = customView.Name;
            Shared = customView.Shared;
            WorkbookId = customView.Workbook.Id;
            OwnerId = customView.Owner.Id;
        }
    }
}
