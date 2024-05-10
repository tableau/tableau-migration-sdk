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

using System.Xml.Serialization;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a paged REST API response.
    /// </summary>
    public abstract class PagedTableauServerResponse<TItem> : TableauServerListResponse<TItem>, IPageInfo
    {
        /// <summary>
        /// Gets or sets the pagination for the response.
        /// </summary>
        [XmlElement("pagination")]
        public Pagination Pagination { get; set; } = new();

        int IPageInfo.PageNumber => Pagination.PageNumber;

        int IPageInfo.PageSize => Pagination.PageSize;

        int IPageInfo.TotalCount => Pagination.TotalAvailable;

        bool IPageInfo.FetchedAllPages
        {
            get
            {
                if (Pagination.TotalAvailable == 0)
                {
                    return true;
                }
                var pagesAvailable = (Pagination.TotalAvailable / Pagination.PageSize) + (Pagination.TotalAvailable % Pagination.PageSize > 0 ? 1 : 0);

                return Pagination.PageNumber >= pagesAvailable;
            }
        }
    }
}
