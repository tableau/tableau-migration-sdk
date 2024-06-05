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

using System.Linq;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal class PythonTypeWriter : IPythonTypeWriter
    {
        internal const string DOTNET_OBJECT = "_dotnet";
        private const string DOTNET_BASE = "_dotnet_base";

        private readonly IPythonDocstringWriter _docWriter;
        private readonly IPythonPropertyWriter _propertyWriter;
        private readonly IPythonMethodWriter _methodWriter;
        private readonly IPythonEnumValueWriter _enumValueWriter;

        public PythonTypeWriter(IPythonDocstringWriter docWriter, 
            IPythonPropertyWriter propertyWriter,
            IPythonMethodWriter methodWriter,
            IPythonEnumValueWriter enumValueWriter)
        {
            _docWriter = docWriter;
            _propertyWriter = propertyWriter;
            _methodWriter = methodWriter;
            _enumValueWriter = enumValueWriter;
        }

        private static string ToParamName(string dotNetTypeName)
        {
            if(dotNetTypeName.StartsWith("I"))
                dotNetTypeName = dotNetTypeName.Substring(1);

            return dotNetTypeName.ToSnakeCase();
        }

        public void Write(IndentingStringBuilder builder, PythonType type)
        {
            var inheritedTypeNames = type.InheritedTypes.Select(t => t.GenericDefinitionName);
            var inheritedTypes = string.Join(", ", inheritedTypeNames);

            using (var classBuilder = builder.AppendLineAndIndent($"class {type.Name}({inheritedTypes}):"))
            {
                _docWriter.Write(classBuilder, type.Documentation);
                classBuilder.AppendLine();

                if(type.EnumValues.Any())
                {
                    foreach(var enumValue in type.EnumValues)
                    {
                        _enumValueWriter.Write(classBuilder, type, enumValue);
                    }
                }
                else
                {
                    var dotNetType = type.DotNetType.Name;

                    classBuilder.AppendLine($"{DOTNET_BASE} = {dotNetType}");
                    classBuilder.AppendLine();

                    var dotNetParam = ToParamName(dotNetType);
                    using (var ctorBuilder = classBuilder.AppendLineAndIndent($"def __init__(self, {dotNetParam}: {dotNetType}) -> None:"))
                    {
                        var ctorDoc = new PythonDocstring($"Creates a new {type.Name} object.", "None.", (dotNetParam, $"A {dotNetType} object."));
                        _docWriter.Write(ctorBuilder, ctorDoc);

                        ctorBuilder.AppendLine($"self.{DOTNET_OBJECT} = {dotNetParam}");
                        ctorBuilder.AppendLine();
                    }

                    foreach (var property in type.Properties)
                    {
                        _propertyWriter.Write(classBuilder, type, property);
                    }

                    foreach (var method in type.Methods)
                    {
                        _methodWriter.Write(classBuilder, type, method);
                    }
                }
            }
        }
    }
}
