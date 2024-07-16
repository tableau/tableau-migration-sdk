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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.JsonConverters.SerializableObjects
{
    /// <summary>
    /// Represents a collection of serializable entries, organized by a string key and a list of <see cref="SerializableManifestEntry"/> as the value.
    /// This class extends <see cref="Dictionary{TKey, TValue}"/> to facilitate serialization and deserialization of migration manifest entries.
    /// </summary>
    public class SerializableEntryCollection : Dictionary<string, List<SerializableManifestEntry>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableEntryCollection"/> class.
        /// </summary>
        public SerializableEntryCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableEntryCollection"/> class with an existing dictionary of entries.
        /// </summary>
        /// <param name="entries">The dictionary containing the initial entries for the collection.</param>
        public SerializableEntryCollection(Dictionary<string, List<SerializableManifestEntry>> entries)
        {
            foreach (var entry in entries)
            {
                Add(entry.Key, entry.Value);
            }
        }
    }
}
