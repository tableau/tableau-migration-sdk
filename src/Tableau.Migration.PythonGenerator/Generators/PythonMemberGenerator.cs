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
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Tableau.Migration.PythonGenerator.Config;
using Dotnet = Tableau.Migration.PythonGenerator.Keywords.Dotnet;
using Py = Tableau.Migration.PythonGenerator.Keywords.Python;

namespace Tableau.Migration.PythonGenerator.Generators
{
    internal abstract class PythonMemberGenerator
    {
        private static readonly PythonTypeReference BOOL = new(Py.Types.BOOL, ImportModule: null, ConversionMode.Direct);
        private static readonly PythonTypeReference INT = new(Py.Types.INT, ImportModule: null, ConversionMode.Direct);
        private static readonly PythonTypeReference STRING = new(Py.Types.STR, ImportModule: null, ConversionMode.Direct);
        private static readonly PythonTypeReference UUID = new(
            Py.Types.UUID,
            ImportModule: Py.Modules.UUID,
            ConversionMode.WrapSerialized,
            DotNetParseFunction: "Guid.Parse",
            ExtraImports: ImmutableArray.Create(
                new PythonTypeReference(Dotnet.Types.GUID, ImportModule: Dotnet.Namespaces.SYSTEM, ConversionMode.Direct)));

        internal static readonly PythonTypeReference LIST_REFERENCE = new PythonTypeReference(
            Dotnet.Types.LIST,
            ImportModule: Dotnet.Namespaces.SYSTEM_COLLECTIONS_GENERIC,
            ConversionMode: ConversionMode.Direct,
            ImportAlias: Dotnet.TypeAliases.LIST);

        internal static readonly PythonTypeReference HASH_SET_REFERENCE = new PythonTypeReference(
            Dotnet.Types.HASH_SET,
            ImportModule: Dotnet.Namespaces.SYSTEM_COLLECTIONS_GENERIC,
            ConversionMode: ConversionMode.Direct,
            ImportAlias: Dotnet.TypeAliases.HASH_SET);

        private static readonly PythonTypeReference STRING_REFERENCE = new PythonTypeReference(
            Dotnet.Types.STRING,
            ImportModule: Dotnet.Namespaces.SYSTEM,
            ConversionMode: ConversionMode.Direct,
            ImportAlias: Dotnet.TypeAliases.STRING);

        private static readonly PythonTypeReference EXCEPTION = new(
            Dotnet.Namespaces.SYSTEM_EXCEPTION,
            Dotnet.Namespaces.SYSTEM,
            ConversionMode.Direct);
        
        private static readonly PythonTypeReference TIME_ONLY = new(
            Py.Types.TIME,
            ImportModule: Py.Modules.DATETIME,
            ConversionMode.WrapTimeOnly,
            DotNetParseFunction: "TimeOnly.Parse",
            ExtraImports: ImmutableArray.Create(
                new PythonTypeReference(Dotnet.Types.TIME_ONLY, ImportModule: Dotnet.Namespaces.SYSTEM, ConversionMode.Direct)));
        
        private readonly PythonGeneratorOptions _options;

        protected PythonMemberGenerator(IOptions<PythonGeneratorOptions> options)
        {
            _options = options.Value;
        }

        protected bool IgnoreMember(ITypeSymbol type, ISymbol member)
        {
            var typeHints = _options.Hints.ForType(type);
            if (typeHints is null)
            {
                return false;
            }

            return typeHints.ExcludeMembers.Any(m => string.Equals(m, member.Name, StringComparison.Ordinal));
        }

        protected ImmutableArray<PythonTypeReference>? GetGenericTypes(ITypeSymbol t)
        {
            if (t is INamedTypeSymbol nt)
            {
                return nt.TypeArguments.Select(ToPythonType).ToImmutableArray();
            }

            if (t is IArrayTypeSymbol at)
            {
                var pyType = ToPythonType(at.ElementType);
                return ImmutableArray.Create(pyType);
            }

            return null;
        }

        protected ImmutableArray<ITypeSymbol>? GetDotnetGenericTypes(ITypeSymbol t)
        {
            if (t is INamedTypeSymbol nt)
            {
                return nt.TypeArguments;
            }

            if (t is IArrayTypeSymbol at)
            {
                return ImmutableArray.Create(at.ElementType);
            }
            return null;
        }

        protected PythonTypeReference ToPythonType(ITypeSymbol t)
        {
            if (t.Kind is SymbolKind.TypeParameter)
            {
                return PythonTypeReference.ForGenericType(t);
            }

            switch (t.Name)
            {
                case "bool":
                case nameof(Boolean):
                    return BOOL;
                case nameof(Exception):
                    return EXCEPTION;
                case nameof(Guid):
                    return UUID;
                case nameof(IList<int>):
                    return new(
                        Py.Types.LIST_WRAPPED,
                        ImportModule: Py.Modules.TYPING,
                        ConversionMode.WrapMutableCollection,
                        GenericTypes: GetGenericTypes(t),
                        ExtraImports: ImmutableArray.Create(LIST_REFERENCE),
                        DotnetTypes: GetDotnetGenericTypes(t));
                case nameof(HashSet<int>):
                    return new(
                        Py.Types.SET,
                        ImportModule: Py.Modules.TYPING,
                        ConversionMode.WrapMutableCollection,
                        GenericTypes: GetGenericTypes(t),
                        ExtraImports: ImmutableArray.Create(HASH_SET_REFERENCE),
                        DotnetTypes: GetDotnetGenericTypes(t));
                case nameof(ISet<int>):
                    return new(
                        Py.Types.SEQUENCE,
                        ImportModule: Py.Modules.TYPING,
                        ConversionMode.WrapMutableCollection,
                        GenericTypes: GetGenericTypes(t),
                        ExtraImports: ImmutableArray.Create(LIST_REFERENCE, STRING_REFERENCE, HASH_SET_REFERENCE),
                        DotnetTypes: GetDotnetGenericTypes(t));

                case nameof(ImmutableArray<int>):
                case nameof(IImmutableList<int>):
                case nameof(IReadOnlyList<int>):
                case nameof(IEnumerable<int>):
                    return new(
                        Py.Types.SEQUENCE,
                        ImportModule: Py.Modules.TYPING,
                        ConversionMode.WrapImmutableCollection,
                        WrapType: "list",
                        GenericTypes: GetGenericTypes(t));
                case Dotnet.Types.INT:
                case nameof(Int32):
                case Dotnet.Types.LONG:
                case nameof(Int64):
                    return INT;
                case nameof(Nullable):
                    return GetGenericTypes(t)!.Value.Single();
                case Dotnet.Types.STRING_SIMPLIFIED:
                case nameof(String):
                    return STRING;
                case nameof(TimeOnly):
                    return TIME_ONLY;
                default:
                    if (t is IArrayTypeSymbol symbol)
                    {
                        return new(
                            Py.Types.SEQUENCE,
                            ImportModule: Py.Modules.TYPING,
                            ConversionMode.WrapArray,
                            WrapType: "list",
                            GenericTypes: GetGenericTypes(t),
                            ExtraImports: ImmutableArray.Create(LIST_REFERENCE),
                            DotnetTypes: GetDotnetGenericTypes(t));
                    }
                    else
                    {
                        return PythonTypeReference.ForDotNetType(t);
                    }
            }
        }

    }
}
