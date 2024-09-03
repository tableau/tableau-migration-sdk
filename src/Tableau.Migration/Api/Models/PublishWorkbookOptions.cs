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
using System.IO;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for API client workbook publish options. 
    /// </summary>
    public class PublishWorkbookOptions : PublishContentWithFileOptions, IPublishWorkbookOptions
    {
        ///<inheritdoc/>
        public string Name { get; }

        ///<inheritdoc/>
        public string Description { get; }
        
        ///<inheritdoc/>
        public bool ShowTabs { get; }

        ///<inheritdoc/>
        public bool EncryptExtracts { get; }

        ///<inheritdoc/>
        public Guid? ThumbnailsUserId { get; }

        ///<inheritdoc/>
        public bool SkipConnectionCheck { get; } = true;

        ///<inheritdoc/>
        public bool Overwrite { get; } = true;

        ///<inheritdoc/>
        public Guid ProjectId { get; }

        ///<inheritdoc/>
        public IEnumerable<string> HiddenViewNames { get; }

        /// <summary>
        /// Creates a new <see cref="PublishWorkbookOptions"/> instance.
        /// </summary>
        /// <param name="workbook">The publishable workbook information.</param>
        /// <param name="file">The workbook file as a <see cref="Stream"/></param>
        /// <param name="fileType">The type of workbook file.</param>
        public PublishWorkbookOptions(
            IPublishableWorkbook workbook, 
            Stream file, 
            string fileType = WorkbookFileTypes.Twbx) 
                 : base(
                  file,
                  workbook.File.OriginalFileName,
                  fileType)
        {
            Name = workbook.Name;
            Description = workbook.Description;
            ShowTabs = workbook.ShowTabs;
            EncryptExtracts = workbook.EncryptExtracts;
            ThumbnailsUserId = workbook.ThumbnailsUserId;
            ProjectId = ((IContainerContent)workbook).Container.Id;            
            HiddenViewNames = workbook.HiddenViewNames;
        }
    }
}
