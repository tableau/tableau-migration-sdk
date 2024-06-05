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

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Tableau.Migration.PythonGenerator.Config;

namespace Tableau.Migration.PythonGenerator.Generators
{
    internal sealed class PythonEnumValueGenerator : PythonMemberGenerator, IPythonEnumValueGenerator
    {
        private readonly IPythonDocstringGenerator _docGenerator;

        public PythonEnumValueGenerator(IPythonDocstringGenerator docGenerator,
            IOptions<PythonGeneratorOptions> options) 
            : base(options)
        {
            _docGenerator = docGenerator;
        }

        public ImmutableArray<PythonEnumValue> GenerateEnumValues(INamedTypeSymbol dotNetType)
        {
            if(!dotNetType.IsAnyEnum())
            {
                return ImmutableArray<PythonEnumValue>.Empty;
            }

            var results = ImmutableArray.CreateBuilder<PythonEnumValue>();

            foreach (var dotNetMember in dotNetType.GetMembers())
            {
                if (!(dotNetMember is IFieldSymbol dotNetField) ||
                    !dotNetField.IsConst ||
                    IgnoreMember(dotNetType, dotNetField))
                {
                    continue;
                }

                var value = dotNetField.ConstantValue!;
                var docs = _docGenerator.Generate(dotNetField);

                var pyValue = new PythonEnumValue(dotNetField.Name.ToConstantCase(), value, docs);

                results.Add(pyValue);
            }

            return results.ToImmutable();
        }
    }
}
