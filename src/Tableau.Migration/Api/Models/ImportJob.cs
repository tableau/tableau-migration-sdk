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
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class ImportJob : IImportJob
    {
        public Guid Id { get; }
        public string Type { get; }
        public DateTime CreatedAtUtc { get; }
        public int ProgressPercentage { get; }

        /// <summary>
        /// Creates a new <see cref="ImportJob"/> instance.
        /// </summary>
        /// <param name="response">The REST API user import job response.</param>
        public ImportJob(ImportJobResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(response.Item.Id, () => response.Item.Id);

            Type = Guard.AgainstNull(response.Item.Type, () => response.Item.Type);

            CreatedAtUtc = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.CreatedAt, () => response.Item.CreatedAt).ParseFromIso8601();

            ProgressPercentage = response.Item.Progress;
        }
    }
}
