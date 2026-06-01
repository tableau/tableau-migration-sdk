//
//  Copyright (c) 2026, Salesforce, Inc.
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
using Microsoft.Extensions.Options;
using Tableau.Migration.PythonGenerator.Config;

namespace Tableau.Migration.PythonGenerator.Generators
{
    internal sealed class PythonTypeGenerator : IPythonTypeGenerator
    {
        private readonly PythonGeneratorOptions _options;
        private readonly IPythonPropertyGenerator _propertyGenerator;
        private readonly IPythonMethodGenerator _methodGenerator;
        private readonly IPythonEnumValueGenerator _enumValueGenerator;
        private readonly IPythonDocstringGenerator _docGenerator;

        public PythonTypeGenerator(IOptions<PythonGeneratorOptions> options,
            IPythonPropertyGenerator propertyGenerator,
            IPythonMethodGenerator methodGenerator,
            IPythonEnumValueGenerator enumValueGenerator,
            IPythonDocstringGenerator docGenerator)
        {
            _options = options.Value;
            _propertyGenerator = propertyGenerator;
            _methodGenerator = methodGenerator;
            _enumValueGenerator = enumValueGenerator;
            _docGenerator = docGenerator;
        }

        private bool HasInterface(INamedTypeSymbol type, INamedTypeSymbol test)
        {
            foreach (var childInterface in type.AllInterfaces)
            {
                if (string.Equals(childInterface.ToDisplayString(), test.ToDisplayString(), System.StringComparison.Ordinal))
                {
                    return true;
                }

                if (HasInterface(childInterface, test))
                {
                    return true;
                }
            }

            return false;
        }

        private (ImmutableArray<PythonTypeReference> InheritedTypes, ImmutableArray<INamedTypeSymbol> ExcludedInterfaces)
            GenerateInheritedTypes(ImmutableHashSet<string> dotNetTypeNames, INamedTypeSymbol dotNetType)
        {
            var inheritedTypes = ImmutableArray.CreateBuilder<PythonTypeReference>();
            
            var typeHint = _options.Hints.ForType(dotNetType);
            bool IgnoreInheritedType(INamedTypeSymbol t)
                => typeHint is not null && typeHint.ExcludeInheritedTypes.Contains(t.Name);

            if (dotNetType.TypeArguments.Any())
            {
                var genericTypes = dotNetType.TypeArguments
                    .Select(PythonTypeReference.ForGenericType)
                    .ToImmutableArray();

                var extraImports = ImmutableArray.Create(
                    new PythonTypeReference("TypeVar", "typing", ConversionMode.Direct),
                    new PythonTypeReference("_generic_wrapper", "tableau_migration.migration", ConversionMode.Direct)
                );

                inheritedTypes.Add(new("Generic", "typing", ConversionMode.Direct, genericTypes, ExtraImports: extraImports));
            }

            if (dotNetType.IsOrdinalEnum())
            {
                inheritedTypes.Add(new("IntEnum", "enum", ConversionMode.Direct));
            }
            else if (dotNetType.IsStringEnum())
            {
                inheritedTypes.Add(new("StrEnum", "migration_enum", ConversionMode.Direct));
            }
            else if (!dotNetType.IsValueType && dotNetType.BaseType is not null)
            {
                switch(dotNetType.BaseType.Name)
                {
                    case nameof(Object):
                    case nameof(ValueType):
                        break;
                    default:
                        if(!IgnoreInheritedType(dotNetType.BaseType))
                        {
                            inheritedTypes.Add(PythonTypeReference.ForDotNetType(dotNetType.BaseType));
                        }
                        break;
                }
            }

            var interfaces = new List<INamedTypeSymbol>();
            var excludedInterfaces = ImmutableArray.CreateBuilder<INamedTypeSymbol>();

            foreach (var interfaceType in dotNetType.AllInterfaces)
            {
                if (dotNetTypeNames.Contains(interfaceType.ConstructedFrom.ToDisplayString()) &&
                    !IgnoreInheritedType(interfaceType))
                {
                    interfaces.Add(interfaceType);
                }
                else
                {
                    excludedInterfaces.Add(interfaceType);
                }
            }

            // Remove interfaces implemented by other inherited interfaces,
            // to reduce multi-inheritance complexity.
            foreach (var interfaceType in interfaces.ToImmutableArray())
            {
                if (interfaces.Any(i => HasInterface(i, interfaceType)))
                {
                    interfaces.Remove(interfaceType);
                }
            }

            inheritedTypes.AddRange(interfaces.Select(PythonTypeReference.ForDotNetType));

            return (inheritedTypes.ToImmutable(), excludedInterfaces.ToImmutable());
        }

        public PythonType Generate(ImmutableHashSet<string> dotNetTypeNames, INamedTypeSymbol dotNetType)
        {
            (var inheritedTypes, var excludedInterfaces) = GenerateInheritedTypes(dotNetTypeNames, dotNetType);

            var properties = _propertyGenerator.GenerateProperties(dotNetType);
            var methods = _methodGenerator.GenerateMethods(dotNetType);
            var enumValues = _enumValueGenerator.GenerateEnumValues(dotNetType);

            // arg docs at the type level would be for record properties.
            var docs = _docGenerator.Generate(dotNetType, ignoreArgs: true);

            var typeRef = PythonTypeReference.ForDotNetType(dotNetType);

            return new(typeRef.Name, typeRef.ImportModule!, inheritedTypes,
                properties, methods, enumValues,
                docs, dotNetType, excludedInterfaces);
        }
    }
}
