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
using System.Collections.Immutable;
using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for an API client job model.
    /// </summary>
    public interface IJob : IRestIdentifiable, IEquatable<IJob>
    {
        /// <summary>
        /// Gets the job's type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the job's created timestamp.
        /// </summary>
        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the job's updated timestamp.
        /// </summary>
        DateTime? UpdatedAtUtc { get; }

        /// <summary>
        /// Gets the job's completed timestamp.
        /// </summary>
        DateTime? CompletedAtUtc { get; }

        /// <summary>
        /// Gets the job's progress percentage.
        /// </summary>
        int ProgressPercentage { get; }

        /// <summary>
        /// Gets the job's finish code.
        /// </summary>
        int FinishCode { get; }

        /// <summary>
        /// Gets the job's status notes.
        /// </summary>
        IImmutableList<IStatusNote> StatusNotes { get; }
    }
}
