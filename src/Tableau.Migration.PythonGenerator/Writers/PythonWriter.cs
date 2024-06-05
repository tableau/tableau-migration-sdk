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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tableau.Migration.PythonGenerator.Config;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonWriter : PythonWriterBase, IPythonWriter
    {
        private readonly IPythonTypeWriter _typeWriter;
        private readonly IPythonTestWriter _testWriter;
        private readonly PythonGeneratorOptions _options;

        public PythonWriter(IPythonTypeWriter typeWriter, IPythonTestWriter testWriter,
            IOptions<PythonGeneratorOptions> options)
        {
            _typeWriter = typeWriter;
            _testWriter = testWriter;
            _options = options.Value;
        }

        private static void WriteTypeVars(IndentingStringBuilder builder, IEnumerable<PythonType> pyTypes)
        {
            var typeVars = pyTypes
                .SelectMany(pt => pt.InheritedTypes)
                .SelectMany(it => it.GenericTypes ?? ImmutableArray<PythonTypeReference>.Empty)
                .Select(gt => gt.Name)
                .Distinct()
                .Order()
                .ToImmutableArray();

            if (typeVars.Any())
            {
                foreach (var typeVar in typeVars)
                {
                    builder.AppendLine($"{typeVar} = TypeVar(\"{typeVar}\")");
                }

                builder.AppendLine();
            }
        }

        private void WriteTypeAndDependencies(IndentingStringBuilder builder, PythonType type,
            IReadOnlyDictionary<string, PythonType> moduleTypes, HashSet<PythonType> writtenTypes, HashSet<PythonType> cycleReferences)
        {
            if (cycleReferences.Contains(type))
            {
                throw new Exception("Type dependency cycle detected. Consider implementing ordering or stubbing.");
            }

            if (writtenTypes.Contains(type))
            {
                return;
            }

            cycleReferences.Add(type);

            var refTypes = type.GetAllTypeReferences().SelectMany(t => t.UnwrapGenerics());
            foreach (var refType in refTypes)
            {
                if (!string.Equals(type.Module, refType.ImportModule, StringComparison.Ordinal) ||
                    string.Equals(type.Name, refType.Name, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!moduleTypes.TryGetValue(refType.Name, out var dependencyType))
                {
                    continue;
                }

                WriteTypeAndDependencies(builder, dependencyType, moduleTypes, writtenTypes, cycleReferences);
            }

            cycleReferences.Remove(type);

            _typeWriter.Write(builder, type);
            writtenTypes.Add(type);
        }

        private void WriteModuleTypes(IndentingStringBuilder builder, IEnumerable<PythonType> moduleTypes)
        {
            var moduleTypesByName = moduleTypes.ToImmutableDictionary(t => t.Name);
            var writtenTypes = new HashSet<PythonType>();

            foreach (var typeToWrite in moduleTypes.OrderBy(t => t.Name))
            {
                var cycleReferences = new HashSet<PythonType>();
                WriteTypeAndDependencies(builder, typeToWrite, moduleTypesByName, writtenTypes, cycleReferences);
            }
        }

        private async Task WritePublicAliasesAsync(IEnumerable<PythonType> moduleTypes, CancellationToken cancel)
        {
            var initFilePath = Path.Combine(_options.OutputPath, "__init__.py");
            await using var segment = await GeneratedPythonSegment.OpenAsync(initFilePath, cancel);

            foreach (var type in moduleTypes.OrderBy(x => x.Module).ThenBy(x => x.Name))
            {
                segment.StringBuilder.AppendLine($"from {type.Module} import {type.Name} as {type.DotNetType.Name} # noqa: E402, F401");
            }
        }

        public async ValueTask WriteAsync(PythonTypeCache pyTypeCache, CancellationToken cancel)
        {
            var moduleGroups = pyTypeCache.Types
                .GroupBy(p => p.Module);

            foreach (var moduleGroup in moduleGroups)
            {
                var pyFileName = moduleGroup.Key.Replace("tableau_migration.", "") + ".py";
                var pyFilePath = Path.Combine(_options.OutputPath, pyFileName);

                await using var segment = await GeneratedPythonSegment.OpenAsync(pyFilePath, cancel);

                WriteImports(segment.StringBuilder, moduleGroup.Key, moduleGroup);

                WriteTypeVars(segment.StringBuilder, moduleGroup);

                //Write types in dependency order.
                WriteModuleTypes(segment.StringBuilder, moduleGroup);
            }

            await WritePublicAliasesAsync(pyTypeCache.Types, cancel);

            await _testWriter.WriteAsync(pyTypeCache, cancel);
        }
    }
}
