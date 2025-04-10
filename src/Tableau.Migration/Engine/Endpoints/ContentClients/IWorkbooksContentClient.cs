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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Endpoints.ContentClients
{
    /// <summary>
    /// Interface for a client that can interact with workbooks.
    /// </summary>
    public interface IWorkbooksContentClient : IContentClient<IWorkbook>
    {
        /// <summary>
        /// Gets the views for a workbook.
        /// </summary>
        /// <param name="workbookId">The Workbook Id to get views for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>Collection of views for the workbook.</returns>
        Task<IResult<IImmutableList<IView>>> GetViewsForWorkbookIdAsync(Guid workbookId, CancellationToken cancel);
    }
}
