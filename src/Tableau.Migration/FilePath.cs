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
using System.Collections.Immutable;
using System.IO;
using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration
{
    internal record FilePath
    {
        // This is not an exhaustive list, just common extensions that are "zippy".
        internal static readonly IImmutableSet<string> ZipExtensions = ImmutableHashSet.Create(
            StringComparer.OrdinalIgnoreCase,
            WorkbookFileTypes.Twbx,
            DataSourceFileTypes.Tdsx,
            "zip",
            "7z",
            "gz",
            "rar"
        );

        public readonly string FileName;
        public readonly string? Extension;
        public readonly bool? IsZipFile;

        public FilePath(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            Extension = Path.GetExtension(filePath)?.TrimStart('.');

            if (!String.IsNullOrWhiteSpace(Extension))
                IsZipFile = ZipExtensions.Contains(Extension);
        }
    }
}
