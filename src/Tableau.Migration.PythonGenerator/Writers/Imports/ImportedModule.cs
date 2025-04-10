//
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

using System.Collections.Generic;
using System.Linq;

namespace Tableau.Migration.PythonGenerator.Writers.Imports
{
    internal class ImportedModule
    {
        public ImportedModule(string name, HashSet<ImportedType> types)
        {
            Name = name;
            Types = types;
        }
        public ImportedModule(string name, HashSet<string> typeNames)
        {
            Name = name;
            Types = typeNames.Select(tn=> new ImportedType(tn)).ToHashSet();
        }

        public ImportedModule(string name, ImportedType type)
        {
            Name = name;
            Types = [type];
        }

        public ImportedModule(string name, string typeName)
        {
            Name = name;
            Types = [new ImportedType(typeName)];
        }

        public string Name { get; set; }

        public HashSet<ImportedType> Types { get; set; }
    }
}
