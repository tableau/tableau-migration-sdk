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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Interface for an extract refresh response type.
    /// </summary>
    public interface IExtractRefreshType : IRestIdentifiable, IWithWorkbookReferenceType, IWithDataSourceReferenceType
    {
        /// <summary>
        /// Gets or sets the priority for the response.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the type for the response.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the consecutive failed count for the response.
        /// </summary>
        public int ConsecutiveFailedCount { get; set; }
    }

    /// <summary>
    /// Interface for an extract refresh response type.
    /// </summary>
    public interface IExtractRefreshType<TWorkbook, TDataSource> : IExtractRefreshType
        where TWorkbook : IRestIdentifiable
        where TDataSource : IRestIdentifiable
    { 
        /// <summary>
        /// Gets or sets the workbook for the response.
        /// </summary>
        new TWorkbook? Workbook { get; set; }

        /// <summary>
        /// Gets or sets the data source for the response.
        /// </summary>
        new TDataSource? DataSource { get; set; }
    }
}