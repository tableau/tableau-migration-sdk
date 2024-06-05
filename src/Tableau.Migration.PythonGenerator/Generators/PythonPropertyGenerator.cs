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
            var results = ImmutableArray.CreateBuilder<PythonProperty>();

            foreach (var dotNetMember in dotNetType.GetMembers())
            {
                if(dotNetMember.IsStatic || 
                    !(dotNetMember is IPropertySymbol dotNetProperty) || 
                    IgnoreMember(dotNetType, dotNetProperty) ||
                    IGNORED_PROPERTIES.Contains(dotNetProperty.Name))
                {
                    continue;
                }

                var type = ToPythonType(dotNetProperty.Type);
                var docs = _docGenerator.Generate(dotNetProperty);

                var pyProperty = new PythonProperty(dotNetProperty.Name.ToSnakeCase(), type, 
                    !dotNetProperty.IsWriteOnly,
                    !(dotNetProperty.IsReadOnly || (dotNetProperty.SetMethod is not null && dotNetProperty.SetMethod.IsInitOnly)),
                    docs, dotNetProperty);

                results.Add(pyProperty);
            }

            return results.ToImmutable();
        }
    }
}
