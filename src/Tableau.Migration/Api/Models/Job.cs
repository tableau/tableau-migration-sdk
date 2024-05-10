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
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class Job : IJob
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Type { get; }

        /// <inheritdoc />
        public DateTime CreatedAtUtc { get; }

        /// <inheritdoc />
        public DateTime? UpdatedAtUtc { get; }

        /// <inheritdoc />
        public DateTime? CompletedAtUtc { get; }

        /// <inheritdoc />
        public int ProgressPercentage { get; }

        /// <inheritdoc />
        public int FinishCode { get; }

        /// <inheritdoc />
        public IImmutableList<IStatusNote> StatusNotes { get; }

        /// <summary>
        /// Creates a new <see cref="Job"/> instance.
        /// </summary>
        /// <param name="response">The REST API job response.</param>
        public Job(JobResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(response.Item.Id, () => response.Item.Id);

            Type = Guard.AgainstNull(response.Item.Type, () => response.Item.Type);

            CreatedAtUtc = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.CreatedAt, () => response.Item.CreatedAt).ParseFromIso8601();

            UpdatedAtUtc = response.Item.UpdatedAt?.ParseFromIso8601();
            CompletedAtUtc = response.Item.CompletedAt?.ParseFromIso8601();

            ProgressPercentage = response.Item.Progress;
            FinishCode = response.Item.FinishCode;

            StatusNotes = response.Item.StatusNotes?.Select(s => new StatusNote(s)).ToImmutableArray<IStatusNote>() ?? ImmutableArray<IStatusNote>.Empty;
        }
    }
}
