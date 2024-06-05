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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator.Generators
{
    internal sealed class PythonGenerator : IPythonGenerator
    {
        private readonly IPythonTypeGenerator _typeGenerator;

        public PythonGenerator(IPythonTypeGenerator typeGenerator)
        {
            _typeGenerator = typeGenerator;
        }

        public PythonTypeCache Generate(IEnumerable<INamedTypeSymbol> dotNetTypes)
        {
            var dotNetTypeNames = dotNetTypes.Select(t => t.ToDisplayString()).ToImmutableHashSet();

            var pyTypes = new List<PythonType>(dotNetTypes.Count());

            foreach(var dotnetType in dotNetTypes)
            {
                var pyType = _typeGenerator.Generate(dotNetTypeNames,dotnetType);
                pyTypes.Add(pyType);
            }

            return new PythonTypeCache(pyTypes);
        }
    }
}
