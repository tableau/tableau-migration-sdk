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
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal class GroupSet : MappableContentBase, IGroupSet
    {
        public GroupSet(Guid id, string? name)
        {
            Id = Guard.AgainstDefaultValue(id, () => id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, () => name);
            Location = new(Name);
            ContentUrl = string.Empty;
        }

        public GroupSet(IGroupSetType response)
            : this(response.Id, response.Name)
        { }

        public GroupSet(IGroupSet copy)
            : this(copy.Id, copy.Name)
        { }
    }
}
