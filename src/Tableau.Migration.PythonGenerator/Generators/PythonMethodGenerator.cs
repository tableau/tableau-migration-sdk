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
    internal class PythonMethodGenerator : PythonMemberGenerator, IPythonMethodGenerator
    {
        private static readonly ImmutableHashSet<string> IGNORED_METHODS = new[]
        {
            nameof(object.Equals),
            nameof(object.GetHashCode),
            nameof(object.ToString),
            nameof(IComparable.CompareTo),
            "Deconstruct",
            "PrintMembers",
            "<Clone>$"
        }.ToImmutableHashSet();

        private readonly IPythonDocstringGenerator _docGenerator;

        public PythonMethodGenerator(IPythonDocstringGenerator docGenerator,
            IOptions<PythonGeneratorOptions> options)
            : base(options)
        {
            _docGenerator = docGenerator;
        }

        private PythonTypeReference? GetPythonReturnType(IMethodSymbol dotNetMethod)
        {
            if(dotNetMethod.ReturnsVoid)
            {
                return null;
            }

            return ToPythonType(dotNetMethod.ReturnType);
        }

        private ImmutableArray<PythonMethodArgument> GenerateArguments(IMethodSymbol dotNetMethod)
        {
            var results = ImmutableArray.CreateBuilder<PythonMethodArgument>();

            foreach(var dotNetParam in dotNetMethod.Parameters)
            {
                var pyArgument = new PythonMethodArgument(dotNetParam.Name.ToSnakeCase(), ToPythonType(dotNetParam.Type));
                results.Add(pyArgument);
            }

            return results.ToImmutable();
        }

        public ImmutableArray<PythonMethod> GenerateMethods(INamedTypeSymbol dotNetType)
        {
            var results = ImmutableArray.CreateBuilder<PythonMethod>();

            foreach (var dotNetMember in dotNetType.GetMembers())
            {
                if (!(dotNetMember is IMethodSymbol dotNetMethod) || IgnoreMember(dotNetType, dotNetMethod))
                {
                    continue;
                }

                if(dotNetMethod.MethodKind is not MethodKind.Ordinary || IGNORED_METHODS.Contains(dotNetMethod.Name))
                {
                    continue;
                }

                var docs = _docGenerator.Generate(dotNetMethod);
                var returnType = GetPythonReturnType(dotNetMethod);
                var arguments = GenerateArguments(dotNetMethod);

                var pyMethod = new PythonMethod(dotNetMethod.Name.ToSnakeCase(), returnType, arguments, dotNetMethod.IsStatic, docs, dotNetMethod);

                results.Add(pyMethod);
            }

            return results.ToImmutable();
        }
    }
}
