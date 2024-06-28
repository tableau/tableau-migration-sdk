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
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Tableau.Migration.PythonGenerator.Config;
using Tableau.Migration.PythonGenerator.Writers.Imports;
using Py = Tableau.Migration.PythonGenerator.Keywords.Python;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonTestWriter : PythonWriterBase, IPythonTestWriter
    {
        private readonly PythonGeneratorOptions _options;
        private readonly IPythonClassTestWriter _classTestWriter;
        private readonly string _testDirectoryPath;

        public PythonTestWriter(IOptions<PythonGeneratorOptions> options, IPythonClassTestWriter classTestWriter)
        {
            _options = options.Value;
            _classTestWriter = classTestWriter;
            _testDirectoryPath = Path.Combine(_options.OutputPath, "..", "..", "tests");
        }

        private bool SearchTypeHierarchy(PythonTypeCache typeCache, PythonType startType, Func<PythonType, bool> search)
        {
            if (search(startType))
            {
                return true;
            }

            foreach (var parentTypeRef in startType.InheritedTypes)
            {
                var parentType = typeCache.Find(parentTypeRef);
                if (parentType is not null && SearchTypeHierarchy(typeCache, parentType, search))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TypeHasMethod(PythonTypeCache typeCache, PythonType startType, IMethodSymbol searchMethod)
            => SearchTypeHierarchy(typeCache, startType, t => t.Methods.Any(m => string.Equals(m.DotNetMethod.Name, searchMethod.Name, StringComparison.Ordinal)));

        private bool TypeHasProperty(PythonTypeCache typeCache, PythonType startType, IPropertySymbol searchProperty)
            => SearchTypeHierarchy(typeCache, startType, t => t.Properties.Any(p => string.Equals(p.DotNetProperty.Name, searchProperty.Name, StringComparison.Ordinal)));

        private void AddHierarchyExcludedMemberHints(List<string> excludedMembers, PythonTypeCache typeCache, PythonType type)
        {
            var typeHints = _options.Hints.ForType(type.DotNetType);
            if (typeHints is not null && typeHints.ExcludeMembers.Any())
            {
                excludedMembers.AddRange(typeHints.ExcludeMembers);
            }

            foreach (var inheritedTypeRef in type.InheritedTypes)
            {
                var inheritedType = typeCache.Find(inheritedTypeRef);
                if (inheritedType is not null)
                {
                    AddHierarchyExcludedMemberHints(excludedMembers, typeCache, inheritedType);
                }
            }
        }

        private string BuildExcludedMemberList(PythonTypeCache typeCache, PythonType t)
        {
            var excludedMembers = t.ExcludedInterfaces
                .SelectMany(x => x.GetMembers())
                .Where(m => m is not IMethodSymbol method || (method.MethodKind is MethodKind.Ordinary && !TypeHasMethod(typeCache, t, method)))
                .Where(m => m is not IPropertySymbol p || !TypeHasProperty(typeCache, t, p))
                .Select(x => x.Name)
                .ToList();

            // Async disposable interface doesn't return any members, but we don't implement dispose in our Python wrappers.
            if (t.ExcludedInterfaces.Any(i => string.Equals(i.Name, "IAsyncDisposable", StringComparison.Ordinal)))
            {
                excludedMembers.Add("DisposeAsync");
            }

            AddHierarchyExcludedMemberHints(excludedMembers, typeCache, t);

            if (excludedMembers.Count == 0)
            {
                return "None";
            }

            var memberNames = excludedMembers
                .Distinct()
                .Order()
                .Select(m => $"\"{m}\"");

            return $"[ {string.Join(", ", memberNames)} ]";
        }

        private static void WriteClassTestImports(IndentingStringBuilder builder, PythonTypeCache pyTypeCache)
        {
            var testTypes = pyTypeCache.Types;

            WritePythonImports(builder, testTypes);

            var enumDotNetTyeps = testTypes
                .OrderBy(x => x.Module)
                .Where(x => x.EnumValues.Any())
                .Select(x => x.DotNetType)
                .OrderBy(x => x.ContainingNamespace.ToDisplayString())
                .ThenBy(x => x.Name)
                .ToImmutableArray();

            if (!enumDotNetTyeps.Any())
            {
                return;
            }

            foreach (var enumType in enumDotNetTyeps)
            {
                builder.AppendLine($"from {enumType.ContainingNamespace.ToDisplayString()} import {enumType.Name}");
            }

            builder.AppendLine();
        }

        private void WriteClassCompletenessTestData(IndentingStringBuilder builder, PythonTypeCache pyTypeCache)
        {
            var testClassTypes = pyTypeCache.Types
                .Where(x => !x.EnumValues.Any())
                .OrderBy(x => x.Module)
                .ThenBy(x => x.Name)
                .ToImmutableArray();

            using (var testDataBuilder = builder.AppendLineAndIndent("_generated_class_data = ["))
            {
                for (int i = 0; i < testClassTypes.Length; i++)
                {
                    var type = testClassTypes[i];
                    var suffix = i == testClassTypes.Length - 1 ? string.Empty : ",";

                    testDataBuilder.AppendLine($"({type.Name}, {BuildExcludedMemberList(pyTypeCache, type)}){suffix}");
                }
            }

            builder.AppendLine("]");
            builder.AppendLine();
        }

        private static void WriteEnumCompletenessTestData(IndentingStringBuilder builder, PythonTypeCache pyTypeCache)
        {
            var testEnumTypes = pyTypeCache.Types
                .Where(x => x.EnumValues.Any())
                .OrderBy(x => x.Module)
                .ThenBy(x => x.Name)
                .ToImmutableArray();

            using (var testDataBuilder = builder.AppendLineAndIndent("_generated_enum_data = ["))
            {
                for (int i = 0; i < testEnumTypes.Length; i++)
                {
                    var type = testEnumTypes[i];
                    var suffix = i == testEnumTypes.Length - 1 ? string.Empty : ",";

                    testDataBuilder.AppendLine($"({type.Name}, {type.DotNetType.Name}){suffix}");
                }
            }

            builder.AppendLine("]");
        }

        private async ValueTask WriteWrapperCompletenessTestDataAsync(PythonTypeCache pyTypeCache, CancellationToken cancel)
        {
            var testClassesPath = Path.Combine(_testDirectoryPath, "test_classes.py");
            await using var segment = await GeneratedPythonSegment.OpenAsync(testClassesPath, cancel);

            WriteClassTestImports(segment.StringBuilder, pyTypeCache);
            WriteClassCompletenessTestData(segment.StringBuilder, pyTypeCache);
            WriteEnumCompletenessTestData(segment.StringBuilder, pyTypeCache);
        }

        private async ValueTask WriteClassMemberTests(PythonTypeCache pyTypeCache, CancellationToken cancel)
        {
            var typesToTest = pyTypeCache
                           .Types
                           .Where(x => PythonTestGenerationList.ShouldGenerateTests(x.DotNetType))
                           .ToList();

            if (typesToTest == null || typesToTest.Count == 0)
            {
                return;
            }

            var namespaceGroups = typesToTest.GroupBy(x 
                => x.DotNetType?.ContainingNamespace.ToDisplayString() ?? string.Empty);

            foreach (var namespaceGroup in namespaceGroups)
            {

                var classes = new PythonTypeCache(namespaceGroup.ToArray());

                var moduleGroups = classes.Types.GroupBy(p => p.Module);

                foreach (var moduleGroup in moduleGroups)
                {
                    var pyFileName = moduleGroup.Key.Replace("tableau_migration.", "") + ".py";
                    var pyFilePath = Path.Combine(_testDirectoryPath, $"test_{pyFileName}");

                    await using var segment = await GeneratedPythonSegment.OpenAsync(pyFilePath, cancel);

                    WriteImports(segment.StringBuilder, moduleGroup.Key, moduleGroup);
                    var pythonTypes = moduleGroup.ToImmutableArray();

                    WriteClassTestImports(segment.StringBuilder, new PythonTypeCache(pythonTypes));

                    WriteExtraTestImports(segment.StringBuilder, namespaceGroup.Key, pyTypeCache);

                    foreach (var pythonType in pythonTypes)
                    {
                        _classTestWriter.Write(segment.StringBuilder, pythonType);
                    }
                }
            }
        }

        private static void WriteExtraTestImports(
            IndentingStringBuilder builder,
            string nameSpace,
            PythonTypeCache pyTypeCache)
        {
            builder.AppendLine("# Extra imports for tests.");

            var autoFixtureNamespace = Py.Modules.AUTOFIXTURE;

            var extraTestImports = PythonTestGenerationList.GetExtraImportsByNamespace(nameSpace, pyTypeCache);

            if (!extraTestImports.Any(x => x.Name == autoFixtureNamespace))
            {
                extraTestImports.Add(new ImportedModule(autoFixtureNamespace, new ImportedType(Py.Types.AUTOFIXTURE_TESTBASE)));
            }
            else
            {
                extraTestImports.First(x => x.Name == autoFixtureNamespace).Types.Add(new ImportedType(Py.Types.AUTOFIXTURE_TESTBASE));
            }

            foreach (var moduleImports in extraTestImports)
            {
                WriteModuleImports(builder, moduleImports);
            }
            builder.AppendLine();
        }
        public async ValueTask WriteAsync(PythonTypeCache pyTypeCache, CancellationToken cancel)
        {
            await WriteWrapperCompletenessTestDataAsync(pyTypeCache, cancel);
            await WriteClassMemberTests(pyTypeCache, cancel);
        }
    }
}
