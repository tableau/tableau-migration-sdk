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

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Tableau.Migration.PythonGenerator.Config;

namespace Tableau.Migration.PythonGenerator.Generators
{
    internal sealed class PythonPropertyGenerator : PythonMemberGenerator, IPythonPropertyGenerator
    {
        private static readonly ImmutableHashSet<string> IGNORED_PROPERTIES = new[]
        {
            "EqualityContract"
        }.ToImmutableHashSet();

        private readonly IPythonDocstringGenerator _docGenerator;

        public PythonPropertyGenerator(IPythonDocstringGenerator docGenerator,
            IOptions<PythonGeneratorOptions> options)
            : base(options)
        {
            _docGenerator = docGenerator;
        }

        public ImmutableArray<PythonProperty> GenerateProperties(INamedTypeSymbol dotNetType)
        {
            // Enums don't generate any properties, but "enum values" through IPythonEnumValueGenerator.
            if (dotNetType.IsAnyEnum())
            {
                return ImmutableArray<PythonProperty>.Empty;
            }

            var results = ImmutableArray.CreateBuilder<PythonProperty>();

            // Generate both C# fields and properties as Python properties.
            foreach (var dotNetMember in dotNetType.GetMembers())
            {
                if (IgnoreMember(dotNetType, dotNetMember) || IGNORED_PROPERTIES.Contains(dotNetMember.Name))
                {
                    continue;
                }

                bool hasGetter, hasSetter, isStatic;
                ITypeSymbol dotNetMemberType;
                if (dotNetMember is IPropertySymbol dotNetProperty)
                {
                    dotNetMemberType = dotNetProperty.Type;
                    hasGetter = !dotNetProperty.IsWriteOnly;
                    hasSetter = !(dotNetProperty.IsReadOnly || (dotNetProperty.SetMethod is not null && dotNetProperty.SetMethod.IsInitOnly));
                    isStatic = dotNetProperty.IsStatic;
                }
                else if (dotNetMember is IFieldSymbol dotNetField)
                {
                    dotNetMemberType = dotNetField.Type;
                    hasGetter = true;
                    hasSetter = !dotNetField.IsReadOnly;
                    isStatic = dotNetField.IsStatic;
                }
                else
                {
                    continue;
                }

                var type = ToPythonType(dotNetMemberType);
                var docs = _docGenerator.Generate(dotNetMember);

                var pyProperty = new PythonProperty(dotNetMember.Name.ToSnakeCase(), type,
                    hasGetter, hasSetter, isStatic, docs, dotNetMember, dotNetMemberType);

                results.Add(pyProperty);
            }

            return results.ToImmutable();
        }
    }
}
