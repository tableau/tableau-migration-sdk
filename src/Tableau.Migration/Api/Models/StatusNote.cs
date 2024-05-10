﻿//
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

using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal class StatusNote : IStatusNote
    {
        /// <inheritdoc />
        public string? Type { get; }

        /// <inheritdoc />
        public string? Value { get; }

        /// <inheritdoc />
        public string? Text { get; }

        public StatusNote(JobResponse.JobType.StatusNoteType response)
        {
            Type = response.Type;
            Value = response.Value;
            Text = response.Text;
        }
    }
}
