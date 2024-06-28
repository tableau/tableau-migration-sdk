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
using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Content.Schedules.Server
{
    internal sealed class ServerSchedule : ScheduleBase, IServerSchedule
    {
        public Guid Id { get; }
        public string Name { get; }
        public string ContentUrl { get; } = String.Empty;
        public ContentLocation Location { get; }

        public string Type { get; }
        public string State { get; }
        public string? CreatedAt { get; }
        public string? UpdatedAt { get; }
        public List<IScheduleExtractRefreshTask> ExtractRefreshTasks { get; set; } = [];

        public ServerSchedule(ScheduleResponse response)
            : this(Guard.AgainstNull(response.Item, () => response.Item))
        { }

        public ServerSchedule(IServerScheduleType response)
            : base(response)
        {
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            Location = new(Name);

            Type = Guard.AgainstNullEmptyOrWhiteSpace(response.Type, () => response.Type);
            State = Guard.AgainstNullEmptyOrWhiteSpace(response.State, () => response.State);
            CreatedAt = response.CreatedAt;
            UpdatedAt = response.UpdatedAt;
        }

        public bool Equals(IContentReference? other) => (other as ServerSchedule)?.Id == Id;

        public static IServerSchedule FromServerResponse(ScheduleResponse response)
            => new ServerSchedule(response);
    }
}
