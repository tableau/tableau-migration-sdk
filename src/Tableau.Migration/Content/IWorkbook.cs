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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a workbook content item.
    /// </summary>
    public interface IWorkbook :
        IContentReference,
        IPublishedContent,
        IDescriptionContent,
        IExtractContent,
        IWithTags,
        IContainerContent,
        IMappableContainerContent,
        IPermissionsContent,
        IRequiresOwnerUpdate,
        IWithConnections
    {
        /// <summary>
        /// Gets or sets whether tabs are shown.
        /// </summary>
        bool ShowTabs { get; set; }

        /// <summary>
        /// Gets the file size.
        /// </summary>
        long Size { get; }
    }
}