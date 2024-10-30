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
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonPropertyTestWriter : PythonMemberWriter, IPythonPropertyTestWriter
    {
        private const string DOTNET_OBJ = "dotnet";
        private const string PY_OBJ = "py";

        public void Write(IndentingStringBuilder builder, PythonType type, PythonProperty property)
        {
            var dotNetPropRef = property.IsStatic ? DotNetTypeName(type) : DOTNET_OBJ;

            if (property.Getter)
            {
                using (var getterBuilder = builder.AppendLineAndIndent($"def test_{property.Name}_getter(self):"))
                {
                    BuildTestBody(type, getterBuilder);
                    AddGetterAsserts(getterBuilder, property, dotNetPropRef);
                }

                builder.AppendLine();
            }

            if (property.Setter)
            {
                using (var setterBuilder = builder.AppendLineAndIndent($"def test_{property.Name}_setter(self):"))
                {
                    BuildTestBody(type, setterBuilder);
                    AddSetterAsserts(setterBuilder, property, dotNetPropRef);
                }

                builder.AppendLine();
            }
        }

        private static void BuildTestBody(PythonType type, IndentingStringBuilder builder)
        {
            builder.AppendLine($"{DOTNET_OBJ} = self.create({DotNetTypeName(type)})");
            builder.AppendLine($"{PY_OBJ} = {PythonTypeName(type)}({DOTNET_OBJ})");
        }

        private static void AddSetterAsserts(IndentingStringBuilder builder, PythonProperty property, string dotNetPropRef)
        {
            var dotnetPropValue = $"{dotNetPropRef}.{property.DotNetProperty.Name}";
            var pythonPropValue = $"{PY_OBJ}.{property.Name}";
            
            if (property.IsStatic)
            {
                pythonPropValue = $"{PY_OBJ}.get_{property.Name}()";
            }

            var typeRef = property.Type;

            switch (typeRef.ConversionMode)
            {
                case ConversionMode.WrapImmutableCollection:
                case ConversionMode.WrapMutableCollection:
                case ConversionMode.WrapArray:
                {
                    builder.AppendLine($"assert len({dotnetPropValue}) != 0");
                    builder.AppendLine($"assert len({pythonPropValue}) == len({dotnetPropValue})");
                    builder.AppendLine();

                    builder.AppendLine("# create test data");

                    var dotnetType = property.DotNetPropertyType;

                    var element = dotnetType switch
                    {
                        INamedTypeSymbol dotnetNameType => dotnetNameType.TypeArguments.First().Name,
                        IArrayTypeSymbol dotnetArrayType => dotnetArrayType.ElementType.Name,
                        _ => throw new InvalidOperationException($"{dotnetType} is not supported.")
                    };

                    builder.AppendLine($"dotnetCollection = DotnetList[{element}]()");

                    for (var i = 1; i < 3; i++)
                    {
                        builder = builder.AppendLine($"dotnetCollection.Add(self.create({element}))");
                    }

                    var collectionWrapExp = ToPythonType(typeRef, "dotnetCollection");
                    builder.AppendLine($"testCollection = {collectionWrapExp}");
                    builder.AppendLine();

                    builder.AppendLine("# set property to new test value");
                    
                    if (property.IsStatic)
                    {
                        builder.AppendLine($"{PY_OBJ}.set_{property.Name}(testCollection)");
                    }
                    else
                    {
                        builder.AppendLine($"{pythonPropValue} = testCollection");
                    }
                    builder.AppendLine();

                    builder.AppendLine("# assert value");
                    builder.AppendLine($"assert len({pythonPropValue}) == len(testCollection)");

                    break;
                }
                default:
                {
                    builder.AppendLine();

                    builder.AppendLine("# create test data");

                    var dotnetType = (INamedTypeSymbol)property.DotNetPropertyType;

                    if (!dotnetType.IsGenericType)
                    {
                        builder.AppendLine($"testValue = self.create({dotnetType.Name})");
                    }
                    else
                    {
                        var args = string.Join(", ", dotnetType.TypeArguments.Select(x => x.Name));
                        builder.AppendLine($"testValue = self.create({dotnetType.OriginalDefinition.Name}[{args}])");
                    }

                    builder.AppendLine();

                    var wrapExp = ToPythonType(typeRef, "testValue");
                    builder.AppendLine("# set property to new test value");
                    if(property.IsStatic)
                    {
                        builder.AppendLine($"{PY_OBJ}.set_{property.Name}({wrapExp})");
                    }
                    else
                    {
                        builder.AppendLine($"{pythonPropValue} = {wrapExp}");
                    }
                    builder.AppendLine();

                    builder.AppendLine("# assert value");
                    builder.AppendLine($"assert {pythonPropValue} == {wrapExp}");
                    break;
                }
            }
        }

        private static void AddGetterAsserts(IndentingStringBuilder builder, PythonProperty property, string dotNetPropRef)
        {
            var dotnetPropValue = $"{dotNetPropRef}.{property.DotNetProperty.Name}";
            var pythonPropValue = $"{PY_OBJ}.{property.Name}";
            if (property.IsStatic)
            {
                pythonPropValue = $"{PY_OBJ}.get_{property.Name}()";
            }

            var typeRef = property.Type;

            switch (typeRef.ConversionMode)
            {
                case ConversionMode.WrapImmutableCollection:
                case ConversionMode.WrapMutableCollection:
                case ConversionMode.WrapArray:
                    builder.AppendLine($"assert len({dotnetPropValue}) != 0");
                    builder.AppendLine($"assert len({pythonPropValue}) == len({dotnetPropValue})");
                    break;
                case ConversionMode.Enum:
                    var enumWrapExp = ToPythonType(typeRef, dotnetPropValue);
                    builder.AppendLine($"assert {pythonPropValue}.value == ({enumWrapExp}).value");
                    break;
                case ConversionMode.Wrap:
                case ConversionMode.WrapSerialized:
                case ConversionMode.WrapGeneric:
                case ConversionMode.Direct:
                default:
                    var wrapExp = ToPythonType(typeRef, dotnetPropValue);
                    builder.AppendLine($"assert {pythonPropValue} == {wrapExp}");
                    break;
            }
        }
    }
}