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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;

namespace Tableau.Migration.PythonGenerator
{
    internal static class PythonGenerationList
    {
        private static readonly ImmutableHashSet<string> TYPES_TO_GENERATE = ToTypeNameHash(
            typeof(ContentLocation),
            typeof(IContentReference),
            typeof(IResult),
            typeof(MigrationCompletionStatus),

            typeof(MigrationManifestEntryStatus),
            typeof(IMigrationManifestEntry),
            typeof(IMigrationManifestEntryEditor),

            typeof(IConnectionsContent),
            typeof(IContainerContent),
            typeof(IWithDomain),
            typeof(IUsernameContent),
            typeof(IUser),
            typeof(IGroup),
            typeof(IDescriptionContent),
            typeof(IProject),
            typeof(IExtractContent),
            typeof(IPublishedContent),
            typeof(IDataSource),
            typeof(IWorkbook),
            typeof(IConnection),
            typeof(IDataSourceDetails),
            typeof(IView),
            typeof(ITag),
            typeof(IPublishableDataSource),
            typeof(IPublishableWorkbook),
            typeof(IGroupUser),
            typeof(IWorkbookDetails),
            typeof(ILabel),

            typeof(ContentMigrationItem<>),
            typeof(ContentItemPostPublishContext<,>),
            typeof(ContentMappingContext<>),
            typeof(BulkPostPublishContext<>),
            typeof(IContentItemMigrationResult<>),
            typeof(IContentBatchMigrationResult<>),
            typeof(IMigrationActionResult),
            typeof(IPublishableGroup),
            typeof(IWithTags),
            typeof(IWithOwner),

            typeof(AdministratorLevels),
            typeof(ContentPermissions),
            typeof(ExtractEncryptionModes),
            typeof(LabelCategories),
            typeof(LicenseLevels),
            typeof(PermissionsCapabilityModes),
            typeof(PermissionsCapabilityNames),
            typeof(SiteRoles),
            typeof(AuthenticationTypes),
            typeof(DataSourceFileTypes),
            typeof(WorkbookFileTypes),
            typeof(GranteeType),
            typeof(IGranteeCapability),
            typeof(IPermissions),
            typeof(ICapability)
        );

        private static ImmutableHashSet<string> ToTypeNameHash(params Type[] types)
            => types.Select(t => t.FullName!).ToImmutableHashSet();

        private static IEnumerable<INamedTypeSymbol> FindTypesToGenerateForNamespace(INamespaceSymbol ns)
        {
            foreach (var type in ns.GetTypeMembers())
            {
                var typeName = $"{type.ContainingNamespace.ToDisplayString()}.{type.MetadataName}";
                if (TYPES_TO_GENERATE.Contains(typeName))
                {
                    yield return type;
                }
            }

            foreach (var childNamespace in ns.GetNamespaceMembers())
            {
                foreach (var type in FindTypesToGenerateForNamespace(childNamespace))
                {
                    yield return type;
                }
            }
        }

        internal static ImmutableArray<INamedTypeSymbol> FindTypesToGenerate(INamespaceSymbol rootNamespace)
            => [.. FindTypesToGenerateForNamespace(rootNamespace)];
    }
}