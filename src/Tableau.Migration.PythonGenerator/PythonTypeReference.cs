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

namespace Tableau.Migration.PythonGenerator
{
    internal sealed record PythonTypeReference(string Name, string? ImportModule,
        ConversionMode ConversionMode, ImmutableArray<PythonTypeReference>? GenericTypes = null,
        string? WrapType = null, string? DotNetParseFunction = null,
        ImmutableArray<PythonTypeReference>? ExtraImports = null,
        ImmutableArray<ITypeSymbol>? DotnetTypes = null,
        string? ImportAlias = null
        )
    {
        public string GenericDefinitionName
            => GenericTypes is null ? Name : $"{Name}[{string.Join(", ", GenericTypes.Value.Select(g => g.GenericDefinitionName))}]";

        public bool IsExplicitReference => Name.Contains(".");

        public static string ToPythonTypeName(ITypeSymbol dotNetType)
        {
            var typeName = dotNetType.Name;
            if (typeName.StartsWith("I"))
                typeName = typeName.Substring(1);

            return "Py" + typeName;
        }

        public IEnumerable<PythonTypeReference> UnwrapGenerics()
        {
            yield return this;

            if (GenericTypes is not null)
            {
                foreach (var generic in GenericTypes)
                {
                    yield return generic;
                }
            }
        }

        public static PythonTypeReference ForGenericType(ITypeSymbol genericType)
            => new PythonTypeReference(genericType.Name, null, ConversionMode.WrapGeneric);

        public static PythonTypeReference ForDotNetType(ITypeSymbol dotNetType)
        {
            var typeName = ToPythonTypeName(dotNetType);

            var mode = ConversionMode.Wrap;

            ImmutableArray<PythonTypeReference>? genericTypes = null;
            if (dotNetType is INamedTypeSymbol namedType)
            {
                if (namedType.TypeArguments.Any())
                {
                    genericTypes = namedType.TypeArguments.Select(ForGenericType).ToImmutableArray();
                }

                if (namedType.IsOrdinalEnum())
                {
                    mode = ConversionMode.Enum;
                }
            }

            return new(typeName, dotNetType.ToPythonModuleName(), mode, genericTypes);
        }
    }
}
