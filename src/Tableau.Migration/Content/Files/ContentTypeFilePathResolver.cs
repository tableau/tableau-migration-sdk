﻿//
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
using System.IO;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="IContentFilePathResolver"/> implementation that generates content item file store paths by their content type.
    /// </summary>
    public class ContentTypeFilePathResolver : IContentFilePathResolver
    {
        /// <inheritdoc />
        public string ResolveRelativePath<TContent>(TContent contentItem, string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName) ?? string.Empty;

            return contentItem switch
            {
                IDataSource ds => Path.Combine(ContentTypeDirectoryNames.DataSources, GetFileName(ContentTypeFilePrefixes.DataSource, ds.Id, extension)),
                IFlow f => Path.Combine(ContentTypeDirectoryNames.Flows, GetFileName(ContentTypeFilePrefixes.Flow, f.Id, extension)),
                IWorkbook wb => Path.Combine(ContentTypeDirectoryNames.Workbooks, GetFileName(ContentTypeFilePrefixes.Workbook, wb.Id, extension)),
                ICustomView cv => Path.Combine(ContentTypeDirectoryNames.CustomViews, GetFileName(ContentTypeFilePrefixes.CustomView, cv.Id, extension)),
                _ => throw new ArgumentException($"Cannot generate a file store path for content type {typeof(TContent).Name}"),
            };

            static string GetFileName(string prefix, Guid id, string extension) => $"{prefix}-{id:N}{extension}";
        }
    }
}
