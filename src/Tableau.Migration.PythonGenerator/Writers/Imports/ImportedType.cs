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

namespace Tableau.Migration.PythonGenerator.Writers.Imports
{
    internal class ImportedType
    {
        public ImportedType(string name, string? alias = null)
        {
            Name = name;
            Alias = alias;
        }

        public string Name { get; set; }

        public string? Alias { get; set; }

        public string GetNameWithAlias()
        {
            return string.IsNullOrEmpty(Alias) ? Name : $"{Name} as {Alias}";
        }

        public override bool Equals(object? obj)
        {
            return obj is ImportedType importedType &&
                   Name == importedType.Name &&
                   Alias == importedType.Alias;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Alias);
        }
    }
}
