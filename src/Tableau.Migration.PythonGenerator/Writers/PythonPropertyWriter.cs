//
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

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Tableau.Migration.PythonGenerator.Generators;
using Py = Tableau.Migration.PythonGenerator.Keywords.Python;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonPropertyWriter : PythonMemberWriter, IPythonPropertyWriter
    {
        private readonly IPythonDocstringWriter _docWriter;

        public PythonPropertyWriter(IPythonDocstringWriter docWriter)
        {
            _docWriter = docWriter;
        }

        public void Write(IndentingStringBuilder builder, PythonType type, PythonProperty property)
        {
            var typeDeclaration = ToPythonTypeDeclaration(type, property.Type);

            var thisParam = property.IsStatic ? "cls" : "self";

            if (property.Getter)
            {
                string getterPrefix;
                if (property.IsStatic)
                {
                    builder.AppendLine("@classmethod");
                    getterPrefix = "get_";
                }
                else
                {
                    builder.AppendLine("@property");
                    getterPrefix = string.Empty;
                }

                using (var getterBuilder = builder.AppendLineAndIndent($"def {getterPrefix}{property.Name}({thisParam}) -> {typeDeclaration}:"))
                {
                    BuildGetterBody(type, property, getterBuilder);
                }

                builder.AppendLine();
            }

            if (property.Setter)
            {
                string setterPrefix;
                if (property.IsStatic)
                {
                    builder.AppendLine("@classmethod");
                    setterPrefix = "set_";
                }
                else
                {
                    builder.AppendLine($"@{property.Name}.setter");
                    setterPrefix = string.Empty;
                }

                var paramName = "value";
                using (var setterBuilder = builder.AppendLineAndIndent($"def {setterPrefix}{property.Name}({thisParam}, {paramName}: {typeDeclaration}) -> None:"))
                {
                    BuildSetterBody(type, property, setterBuilder, paramName);
                }

                builder.AppendLine();
            }
        }

        private void BuildGetterBody(PythonType type, PythonProperty property, IndentingStringBuilder getterBuilder)
        {
            _docWriter.Write(getterBuilder, property.Documentation);

            var getterExpression = ToPythonType(property.Type, DotNetPropertyInvocation(type, property));

            getterBuilder.AppendLine($"return {getterExpression}");
        }

        private static string DotNetPropertyReference(PythonType type, PythonProperty property)
            => property.IsStatic ? DotNetTypeName(type) : $"self.{PythonTypeWriter.DOTNET_OBJECT}";

        private static string DotNetPropertyInvocation(PythonType type, PythonProperty property)
            => $"{DotNetPropertyReference(type, property)}.{property.DotNetProperty.Name}";

        private static void BuildForLoop(IndentingStringBuilder builder, string condition, Action<IndentingStringBuilder> body)
        {
            var forLoopBuilder = builder.AppendLineAndIndent($"for {condition}:");
            body(forLoopBuilder);
        }

        private static void BuildIfBlock(IndentingStringBuilder builder, string condition, Action<IndentingStringBuilder> body)
        {
            var ifBuilder = builder.AppendLineAndIndent($"if {condition}:");
            body(ifBuilder);
        }

        private static void BuildElseBlock(IndentingStringBuilder builder, Action<IndentingStringBuilder> body)
        {
            var elseBuilder = builder.AppendLineAndIndent($"else:");
            body(elseBuilder);
        }

        private void BuildSetterBody(PythonType type, PythonProperty property, IndentingStringBuilder setterBuilder, string paramName)
        {
            _docWriter.Write(setterBuilder, property.Documentation);

            var conversionMode = property.Type.ConversionMode;
            switch (conversionMode)
            {
                case ConversionMode.WrapMutableCollection:
                case ConversionMode.WrapArray:
                    {
                        var typeRef = property.Type;

                        var dotnetTypes = typeRef.DotnetTypes
                            ?? throw new InvalidOperationException("Dotnet types are necessary for wrapping mutable collections.");
                        var dotnetType = dotnetTypes[0];

                        var collectionTypeAlias = GetCollectionTypeAlias(conversionMode, typeRef.Name);

                        BuildIfBlock(setterBuilder, $"{paramName} is None", (builder) =>
                        {
                            builder.AppendLine($"{DotNetPropertyInvocation(type, property)} = {collectionTypeAlias}[{dotnetType.Name}]()");
                        });

                        BuildElseBlock(setterBuilder, (builder) =>
                        {
                            builder.AppendLine($"dotnet_collection = {collectionTypeAlias}[{dotnetType.Name}]()");
                            // Build for loop inside else block
                            var itemVariableName = "x";

                            BuildForLoop(builder, $"{itemVariableName} in filter(None,{paramName})", (builder) =>
                            {
                                if (typeRef.GenericTypes is null || !typeRef.GenericTypes.Value.Any())
                                {
                                    builder.AppendLine($"dotnet_collection.Add({itemVariableName})");
                                }
                                else if (typeRef.GenericTypes.Value.Length > 1)
                                {
                                    throw new InvalidOperationException("Multi-dimensional collections are not currently supported.");
                                }
                                else
                                {
                                    var element = ToDotNetType(typeRef.GenericTypes.Value[0], itemVariableName, skipNoneCheck: true);
                                    builder.AppendLine($"dotnet_collection.Add({element})");
                                }
                            });

                            if (conversionMode is ConversionMode.WrapArray)
                            {
                                builder.AppendLine($"{DotNetPropertyInvocation(type, property)} = dotnet_collection.ToArray()");
                                return;
                            }
                            builder.AppendLine($"{DotNetPropertyInvocation(type, property)} = dotnet_collection");
                        });
                        break;
                    }
                case ConversionMode.Enum:
                    {
                        var setterExpression = ToDotNetType(property.Type, paramName);
                        setterBuilder.AppendLine($"{DotNetPropertyInvocation(type, property)}.value__ = {property.Type.Name}({setterExpression})");
                        break;
                    }
                default:
                    {
                        var setterExpression = ToDotNetType(property.Type, paramName);
                        setterBuilder.AppendLine($"{DotNetPropertyInvocation(type, property)} = {setterExpression}");
                        break;
                    }
            }

            static string? GetCollectionTypeAlias(ConversionMode conversionMode, string typeRefName)
            {
                string? collectionTypeAlias;
                if (conversionMode is ConversionMode.WrapArray)
                {
                    collectionTypeAlias = PythonMemberGenerator.LIST_REFERENCE.ImportAlias;
                }
                else
                {
                    collectionTypeAlias = typeRefName switch
                    {
                        Py.Types.SEQUENCE => PythonMemberGenerator.HASH_SET_REFERENCE.ImportAlias,
                        _ => PythonMemberGenerator.LIST_REFERENCE.ImportAlias,
                    };
                }

                return collectionTypeAlias;
            }
        }
    }
}
