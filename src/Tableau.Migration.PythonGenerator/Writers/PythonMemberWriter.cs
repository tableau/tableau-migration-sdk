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
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal abstract class PythonMemberWriter
    {
        protected static string ToPythonTypeDeclaration(PythonType currentType, PythonTypeReference typeRef)
        {
            if (currentType.Equals(typeRef))
            {
                return "Self";
            }

            if (typeRef.GenericTypes is null)
            {
                return typeRef.Name;
            }

            return $"{typeRef.Name}[{string.Join(", ", typeRef.GenericTypes.Value.Select(g => ToPythonTypeDeclaration(currentType, g)))}]";
        }

        private static string BuildWrapExpression(string nullTestExpression, string wrapExpression)
            => $"None if {nullTestExpression} is None else {wrapExpression}";

        private static string BuildWrapExpression(string nullTestExpression, string wrapFunction, string wrapExpression)
            => $"None if {nullTestExpression} is None else {wrapFunction}({wrapExpression})";

        private static string BuildArrayWrapExpression(string expression, string? itemName = null)
        {
            var arrayElement = string.IsNullOrEmpty(itemName) ? $"x" : $"{itemName}(x)";
            return $"[] if {expression} is None else [{arrayElement} for x in {expression} if x is not None]";
        }


        protected static string ToPythonType(PythonTypeReference typeRef, string expression)
        {
            var wrapCtor = typeRef.WrapType ?? typeRef.GenericDefinitionName;

            switch (typeRef.ConversionMode)
            {
                case ConversionMode.Wrap:
                    return BuildWrapExpression(expression, wrapCtor, expression);
                case ConversionMode.WrapSerialized:
                    return BuildWrapExpression(expression, wrapCtor, $"{expression}.ToString()");
                case ConversionMode.WrapGeneric:
                    return BuildWrapExpression(expression, "_generic_wrapper", expression);
                case ConversionMode.WrapImmutableCollection:
                case ConversionMode.WrapArray:
                    if (typeRef.GenericTypes is null || !typeRef.GenericTypes.Value.Any())
                    {
                        return BuildWrapExpression(expression, wrapCtor, expression);
                    }

                    if (typeRef.GenericTypes.Value.Length > 1)
                    {
                        throw new InvalidOperationException("Multi-dimensional collections are not currently supported.");
                    }

                    var itemType = typeRef.GenericTypes.Value[0];
                    if (itemType.ConversionMode is ConversionMode.Direct)
                    {
                        return BuildWrapExpression(expression, wrapCtor, expression);
                    }

                    var itemExpression = ToPythonType(itemType, "x");
                    return BuildWrapExpression(expression, wrapCtor, $"({itemExpression}) for x in {expression}");
                case ConversionMode.WrapMutableCollection:
                    if (typeRef.GenericTypes is null || !typeRef.GenericTypes.Value.Any())
                    {
                        return BuildArrayWrapExpression(expression);
                    }

                    if (typeRef.GenericTypes.Value.Length > 1)
                    {
                        throw new InvalidOperationException("Multi-dimensional collections are not currently supported.");
                    }

                    var mutableItemType = typeRef.GenericTypes.Value[0];

                    if (mutableItemType.ConversionMode is ConversionMode.Direct)
                    {
                        return BuildArrayWrapExpression(expression);
                    }

                    return BuildArrayWrapExpression(expression, mutableItemType.Name);
                case ConversionMode.Enum:
                    return BuildWrapExpression(expression, wrapCtor, $"{expression}.value__");
                case ConversionMode.WrapTimeOnly:
                    return BuildWrapExpression(expression, wrapCtor, $"{expression}.Hour, {expression}.Minute, {expression}.Second, {expression}.Millisecond * 1000");
                case ConversionMode.Direct:
                default:
                    return expression;
            }
        }

        protected static string ToDotNetType(PythonTypeReference typeRef, string expression, bool skipNoneCheck = false)
        {
            switch (typeRef.ConversionMode)
            {
                case ConversionMode.Wrap:
                case ConversionMode.WrapImmutableCollection:
                case ConversionMode.WrapMutableCollection:
                case ConversionMode.WrapArray:
                    if (skipNoneCheck)
                        return $"{expression}.{PythonTypeWriter.DOTNET_OBJECT}";
                    else
                        return BuildWrapExpression(expression, $"{expression}.{PythonTypeWriter.DOTNET_OBJECT}");
                case ConversionMode.WrapSerialized:
                    return BuildWrapExpression(expression, typeRef.DotNetParseFunction!, $"str({expression})");
                case ConversionMode.WrapTimeOnly:
                    return BuildWrapExpression(expression, typeRef.DotNetParseFunction!, $"str({expression})");
                case ConversionMode.Direct:
                default:
                    return expression;
            }
        }

        protected static string BuildDotnetGenericTypeConstraintsString(INamedTypeSymbol dotnetType)
        {
            var typeConstraints = dotnetType.TypeParameters.First().ConstraintTypes;
            return string.Join(",", typeConstraints.Select(t => t.Name));
        }
        
        protected static string BuildPythongGenericTypeConstraintsString(INamedTypeSymbol dotnetType)
        {
            var typeConstraints = dotnetType.TypeParameters.First().ConstraintTypes;
            return string.Join(",", typeConstraints.Select(PythonTypeReference.ToPythonTypeName));
        }
    }
}
