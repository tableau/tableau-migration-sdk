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
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator
{
    internal sealed record PythonType(string Name, string Module,
        ImmutableArray<PythonTypeReference> InheritedTypes,
        ImmutableArray<PythonProperty> Properties, ImmutableArray<PythonMethod> Methods, ImmutableArray<PythonEnumValue> EnumValues,
        PythonDocstring? Documentation, INamedTypeSymbol DotNetType, ImmutableArray<INamedTypeSymbol> ExcludedInterfaces)
        : IEquatable<PythonTypeReference>
    {
        public IEnumerable<PythonTypeReference> GetAllTypeReferences()
        {
            //Top-level Dotnet type ref
            if(!DotNetType.IsAnyEnum())
            {
                yield return new PythonTypeReference(DotNetType.Name, ImportModule: DotNetType.ContainingNamespace.ToDisplayString(), ConversionMode.Wrap);
            }

            foreach(var t in InheritedTypes)
            {
                yield return t;
            }

            foreach(var m in GetMemberTypeReferences())
            {
                yield return m;
            }
        }

        public IEnumerable<PythonTypeReference> GetMemberTypeReferences()
        {
            foreach (var p in Properties)
            {
                yield return p.Type;
            }

            foreach (var m in Methods)
            {
                if (m.ReturnType is not null)
                {
                    yield return m.ReturnType;
                }

                foreach (var arg in m.Arguments)
                {
                    yield return arg.Type;
                }
            }
        }

        public bool Equals(PythonTypeReference? other)
        {
            if(other is null)
            {
                return false;
            }

            if(!string.Equals(Name, other.Name, System.StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.Equals(Module, other.ImportModule, System.StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        public bool HasSelfTypeReference()
            => GetMemberTypeReferences().Any(Equals);
    }
}
