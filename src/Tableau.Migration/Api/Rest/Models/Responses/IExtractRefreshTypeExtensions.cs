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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    internal static class IExtractRefreshTypeExtensions
    {
        public static ExtractRefreshContentType GetContentType(this IExtractRefreshType extractRefreshType)
            => extractRefreshType is IWithWorkbookReferenceType { Workbook: not null }
                ? ExtractRefreshContentType.Workbook
                : extractRefreshType is IWithDataSourceReferenceType { DataSource: not null }
                    ? ExtractRefreshContentType.DataSource
                    : ExtractRefreshContentType.Unknown;

        public static Guid GetContentId(this IExtractRefreshType extractRefreshType)
            => (extractRefreshType as IWithWorkbookReferenceType)?.Workbook?.Id ??
                (extractRefreshType as IWithDataSourceReferenceType)?.DataSource?.Id ??
                throw new ArgumentException($"{nameof(extractRefreshType)} must contain a valid workbook or data source ID.");
    }
}
