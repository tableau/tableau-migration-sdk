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
using Tableau.Migration.Content.Schedules;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Interface for an object that can create content reference finders
    /// based on content type.
    /// </summary>
    public interface IContentReferenceFinderFactory
    {
        /// <summary>
        /// Gets or creates a content reference finder for a given content type. 
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <returns>The content reference finder.</returns>
        IContentReferenceFinder<TContent> ForContentType<TContent>()
            where TContent : class, IContentReference;

        /// <summary>
        /// Gets or creates a content reference finder for a given extract refresh content type. 
        /// </summary>
        /// <param name="contentType">The extract refresh content type</param>
        /// <returns>The content reference finder.</returns>
        public IContentReferenceFinder ForExtractRefreshContent(ExtractRefreshContentType contentType)
        {
            if (contentType is ExtractRefreshContentType.DataSource)
                return ForContentType<IDataSource>();

            if (contentType is ExtractRefreshContentType.Workbook)
                return ForContentType<IWorkbook>();

            throw new NotSupportedException($"Content type {contentType} is not supported.");
        }
    }
}
