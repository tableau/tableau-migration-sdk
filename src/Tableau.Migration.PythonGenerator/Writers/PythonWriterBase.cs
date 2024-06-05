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
using Tableau.Migration.PythonGenerator.Writers.Imports;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal abstract class PythonWriterBase
    {
        protected internal static void WriteImports(IndentingStringBuilder builder, string currentModule, IEnumerable<PythonType> pyTypes)
        {
            var typeRefs = new HashSet<PythonTypeReference>();

            foreach (var pyType in pyTypes)
            {
                foreach (var typeRef in pyType.GetAllTypeReferences())
                {
                    foreach (var t in typeRef.UnwrapGenerics())
                    {
                        typeRefs.Add(t);
                    }

                    if (typeRef.ExtraImports is not null)
                    {
                        foreach (var extraImport in typeRef.ExtraImports)
                        {
                            typeRefs.Add(extraImport);
                        }
                    }
                }
            }

            if (pyTypes.Any(t => t.HasSelfTypeReference()))
            {
                typeRefs.Add(new("Self", "typing_extensions", ConversionMode.Direct));
            }

            var pythonRefs = typeRefs.Where(IsPythonReference);
            if (pythonRefs.Any())
            {
                WriteImportSection(builder, currentModule, pythonRefs);
                builder.AppendLine();
            }

            var dotNetRefs = typeRefs.Where(t => !IsPythonReference(t));
            if (dotNetRefs.Any())
            {
                WriteImportSection(builder, currentModule, dotNetRefs);
                builder.AppendLine();
            }
        }

        protected internal static void WritePythonImports(IndentingStringBuilder builder, ImmutableArray<PythonType> pyTypes)
        {
            if (pyTypes.Length == 0)
            {
                return;
            }

            if (pyTypes.Length == 1)
            {
                var typeName = pyTypes.First().Name;
                var module = pyTypes.First().Module;

                builder.AppendLine($"from {module} import {typeName} # noqa: E402, F401");
                return;
            }

            foreach (var moduleBasedGrouping in pyTypes.GroupBy(x => x.Module))
            {
                var typesList = moduleBasedGrouping
                    .OrderBy(x => x.Name)
                    .Select(x => new ImportedType(x.Name))
                    .ToHashSet();
                WriteModuleImports(builder, new ImportedModule(moduleBasedGrouping.Key, typesList));
                builder.AppendLine();
            }

            builder.AppendLine();
        }

        private static bool IsPythonReference(PythonTypeReference r) => string.IsNullOrEmpty(r.ImportModule) || char.IsLower(r.ImportModule[0]);

        private static void WriteImportSection(IndentingStringBuilder builder, string currentModule, IEnumerable<PythonTypeReference> typeRefs)
        {
            var explicitImportModules = typeRefs
                .Where(r => r.IsExplicitReference && r.ImportModule is not null)
                .Select(r => r.ImportModule!)
                .Distinct()
                .Order()
                .ToImmutableArray();

            if (explicitImportModules.Any())
            {
                foreach (var explicitImportModule in explicitImportModules)
                {
                    builder.AppendLine($"import {explicitImportModule} # noqa: E402");
                }

                builder.AppendLine();
            }

            var implicitImportModules = typeRefs
                .Where(r => !r.IsExplicitReference)
                .Where(r => r.ImportModule is not null && !string.Equals(currentModule, r.ImportModule, System.StringComparison.Ordinal))
                .GroupBy(r => r.ImportModule!)
                .OrderBy(n => n.Key);

            foreach (var implicitImportModule in implicitImportModules)
            {
                var typesList = new HashSet<ImportedType>();
                foreach (var kvp in implicitImportModule)
                {
                    typesList.Add(new ImportedType(kvp.Name, kvp.ImportAlias));
                }

                WriteModuleImports(builder, new(implicitImportModule.Key, typesList));
            }
        }

        /// <summary>
        /// Writes imports for all types in a module using a list of type names.
        /// </summary>
        /// <param name="builder">The string builder to use.</param>
        /// <param name="module">The name of the module.</param>
        /// <param name="typeNames">The list of type names</param>
        internal static void WriteModuleImports(IndentingStringBuilder builder, ImportedModule importedModule)
        {
            var module = importedModule.Name;
            var types = importedModule.Types;
            if (types.Count == 0)
            {
                return;
            }

            if (types.Count == 1)
            {
                builder.AppendLine($"from {module} import {types.First().GetNameWithAlias()} # noqa: E402, F401");
                return;
            }

            var indentingBuilder = builder.AppendLineAndIndent($"from {module} import (  # noqa: E402, F401");

            foreach (var typeName in types.SkipLast(1))
            {
                indentingBuilder.AppendLine($"{typeName.GetNameWithAlias()},");
            }

            indentingBuilder.AppendLine($"{types.Last().GetNameWithAlias()}");
            builder.AppendLine(")");
        }
    }
}