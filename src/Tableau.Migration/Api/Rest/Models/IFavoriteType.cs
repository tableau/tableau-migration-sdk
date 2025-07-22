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

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// The interface for a favorite request/response item.
    /// </summary>
    public interface IFavoriteType
    {
        /// <summary>
        /// Gets the favorite's referenced data source, or null if the favorite does not reference a data source.
        /// </summary>
        IRestIdentifiable? DataSource { get; }

        /// <summary>
        /// Gets the favorite's referenced flow, or null if the favorite does not reference a flow.
        /// </summary>
        IRestIdentifiable? Flow { get; }

        /// <summary>
        /// Gets the favorite's referenced project, or null if the favorite does not reference a project.
        /// </summary>
        IRestIdentifiable? Project { get; }

        /// <summary>
        /// Gets the favorite's referenced view, or null if the favorite does not reference a view.
        /// </summary>
        IRestIdentifiable? View { get; }

        /// <summary>
        /// Gets the favorite's referenced workbook, or null if the favorite does not reference a workbook.
        /// </summary>
        IRestIdentifiable? Workbook { get; }

        /// <summary>
        /// Gets the favorite's referenced collection, or null if the favorite does not reference a collection.
        /// </summary>
        IRestIdentifiable? Collection { get; }
    }
}
