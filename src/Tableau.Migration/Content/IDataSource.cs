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
    /// Interface for a data source content item.
    /// </summary>
    public interface IDataSource :
        IContentReference,
        IPublishedContent,
        IDescriptionContent,
        IExtractContent,
        IWithTags,
        IContainerContent,
        IMappableContainerContent,
        IPermissionsContent,
        IRequiresOwnerUpdate,
        IWithConnections,
        IRequiresLabelUpdate
    {
        /// <summary>
        /// Gets whether or not the data source has extracts.
        /// </summary>
        bool HasExtracts { get; }

        /// <summary>
        /// Gets the IsCertified flag for the data source.
        /// Should be updated through a post-publish hook.
        /// </summary>
        bool IsCertified { get; }

        /// <summary>
        /// Gets or sets the UseRemoteQueryAgent flag for the data source.
        /// </summary>
        bool UseRemoteQueryAgent { get; set; }
    }
}
