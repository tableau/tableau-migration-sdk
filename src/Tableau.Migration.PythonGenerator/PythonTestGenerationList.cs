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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.PythonGenerator.Writers.Imports;
using Dotnet = Tableau.Migration.PythonGenerator.Keywords.Dotnet;


namespace Tableau.Migration.PythonGenerator
{
    internal class PythonTestGenerationList
    {
        private static readonly ImmutableHashSet<string> TYPES_TO_EXCLUDE = GenerationListHelper.ToTypeNameHash(

        #region - Tableau.Migration.Engine.Manifest -   

            typeof(IMigrationManifestEntry),

        #endregion

        #region - Tableau.Migration.Engine -

            typeof(ContentMigrationItem<>),

        #endregion

        #region - Tableau.Migration.Engine.Hooks.PostPublish  -

            typeof(ContentItemPostPublishContext<,>),

        #endregion

        #region - Tableau.Migration.Engine.Hooks.Mappings  -

            typeof(ContentMappingContext<>),

        #endregion

        #region - Tableau.Migration.Engine.Hooks.PostPublish  -
            typeof(BulkPostPublishContext<>),

        #endregion

        #region - Tableau.Migration.Engine.Migrators -
            typeof(IContentItemMigrationResult<>),

        #endregion

        #region - Tableau.Migration.Engine.Migrators.Batch -

            typeof(IContentBatchMigrationResult<>),

        #endregion

        #region - Tableau.Migration.Engine.Actions -

            typeof(IMigrationActionResult),

        #endregion

        #region - Tableau.Migration.Api.Rest.Models -

            typeof(AdministratorLevels),
            typeof(ContentPermissions),
            typeof(ExtractEncryptionModes),
            typeof(LabelCategories),
            typeof(LicenseLevels),
            typeof(PermissionsCapabilityModes),
            typeof(PermissionsCapabilityNames),
            typeof(SiteRoles),

        #endregion

        #region - Tableau.Migration.Api.Rest.Models.Types -

            typeof(AuthenticationTypes),
            typeof(DataSourceFileTypes),
            typeof(WorkbookFileTypes),

        #endregion

        #region - Tableau.Migration.Content.Schedules.Cloud - 

            // Excluded because ICloudSchedule needs wrapped but there is nothing in it to test.
            typeof(ICloudSchedule)

        #endregion
        );

        private static readonly ImportedModule IContentReferenceImport = new(
            Dotnet.Namespaces.TABLEAU_MIGRATION,
            new ImportedType(nameof(IContentReference)));

        private static readonly ImportedModule DotnetListImport = new(
            Dotnet.Namespaces.SYSTEM_COLLECTIONS_GENERIC,
            new ImportedType(Dotnet.Types.LIST, Dotnet.TypeAliases.LIST));

        private static readonly Dictionary<string, List<ImportedModule>> NAMESPACE_IMPORTS = new()
        {
            {
                $"{typeof(IUser).Namespace}",
                new List<ImportedModule>()
                {
                    IContentReferenceImport,
                    new(Dotnet.Namespaces.SYSTEM, [new ImportedType(Dotnet.Types.BOOLEAN), new ImportedType(Dotnet.Types.NULLABLE)])
                }
            },
            {
                $"{typeof(IPermissions).Namespace}",
                new List<ImportedModule>()
                {
                    IContentReferenceImport,
                    new(Dotnet.Namespaces.SYSTEM,new ImportedType(Dotnet.Types.NULLABLE)),
                    DotnetListImport
                }
            },
            {
                $"{typeof(ISchedule).Namespace}",
                new List<ImportedModule>()
                {
                    IContentReferenceImport,
                    new(Dotnet.Namespaces.SYSTEM, [new ImportedType(Dotnet.Types.NULLABLE), new ImportedType(Dotnet.Types.TIME_ONLY), new ImportedType(Dotnet.Types.STRING)]),
                    DotnetListImport,
                    new($"{typeof(ExtractRefreshContentType).Namespace}",new ImportedType(nameof(ExtractRefreshContentType)))
                }
            }
        };

        internal static bool ShouldGenerateTests(INamedTypeSymbol? type)
        {
            if (type == null)
                return false;

            var argCount = type.TypeArguments.Length;

            var typeName = $"{type?.ContainingNamespace}.{type?.Name}{(argCount > 0 ? $"`{argCount}" : string.Empty)}";

            return !string.IsNullOrEmpty(typeName) && !TYPES_TO_EXCLUDE.Contains(typeName);
        }

        internal static List<ImportedModule> GetExtraImportsByNamespace(string nameSpace, PythonTypeCache pyTypeCache)
        {
            var types = pyTypeCache
               .Types
               .Where(x
                => x.DotNetType?.ContainingNamespace?.ToDisplayString() != null
                && x.DotNetType.ContainingNamespace.ToDisplayString() == nameSpace)
               .ToArray();

            if (types.Length == 0 || !NAMESPACE_IMPORTS.ContainsKey(nameSpace))
            {
                return [];
            }

            return NAMESPACE_IMPORTS[nameSpace];
        }
    }
}
