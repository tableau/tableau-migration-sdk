//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Linq;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Search
{
    internal static class MissingContentReferenceExtensions
    {
        private static string BuildMissingContentReferenceError(ISharedResourcesLocalizer localizer, string refTypeName, string refUse, string context)
            => string.Format(localizer[SharedResourceKeys.MissingContentReferenceError], refTypeName, refUse, context);

        private static string IdContext(Guid missingSourceId)
            => $"Missing source ID: {missingSourceId}";

        private static string LocationContext(ContentLocation missingSourceLocation)
            => $"Missing source location: {missingSourceLocation}";

        private static string ContentUrlContext(string missingSourceContentUrl)
            => $"Missing source content URL: {missingSourceContentUrl}";

        private static IContentReference ThrowOnMissingContentReference(this IResult<IContentReference> result,
            ISharedResourcesLocalizer localizer, string refTypeName, string refUse, string context)
        {
            if (result.Success)
            {
                return result.Value;
            }

            var errorMsg = BuildMissingContentReferenceError(localizer, refTypeName, refUse, context);
            throw new Exception(errorMsg, result.ErrorsToException());
        }

        internal static IContentReference ThrowOnMissingContentReference<TContentRef>(this IResult<IContentReference> result,
            ISharedResourcesLocalizer localizer, string refUse, ContentLocation missingSourceLocation)
            => result.ThrowOnMissingContentReference(localizer, MigrationPipelineContentType.GetDisplayNameForType(typeof(TContentRef)), refUse,
                LocationContext(missingSourceLocation));

        internal static IContentReference ThrowOnMissingContentReference<TContentRef>(this IResult<IContentReference> result,
            ISharedResourcesLocalizer localizer, string refUse, Guid missingSourceId)
            => result.ThrowOnMissingContentReference(localizer, MigrationPipelineContentType.GetDisplayNameForType(typeof(TContentRef)), refUse,
                IdContext(missingSourceId));

        internal static void ThrowOnMissingContentReferences<TContentRef>(this IReadOnlyCollection<ContentLocation> missingSourceLocations,
            ISharedResourcesLocalizer localizer, string refUse)
        {
            string ctx;
            if(missingSourceLocations.Count < 1)
            {
                return;
            }
            else if(missingSourceLocations.Count == 1)
            {
                ctx = LocationContext(missingSourceLocations.Single());
            }
            else
            {
                ctx = "Missing source locations: " + string.Join(", ", missingSourceLocations);
            }

            var errorMsg = BuildMissingContentReferenceError(localizer, MigrationPipelineContentType.GetDisplayNameForType(typeof(TContentRef)), refUse, ctx);
            throw new Exception(errorMsg);
        }

        internal static void ThrowOnMissingContentReferences(this IReadOnlyCollection<PermissionGranteeGroup> missingSourceGrantees, ISharedResourcesLocalizer localizer)
        {
            string ctx;
            if (missingSourceGrantees.Count < 1)
            {
                return;
            }
            else if (missingSourceGrantees.Count == 1)
            {
                ctx = $"Missing source grantee: {missingSourceGrantees.Single()}";
            }
            else
            {
                ctx = "Missing source grantees: " + string.Join(", ", missingSourceGrantees);
            }

            var errorMsg = BuildMissingContentReferenceError(localizer, "grantee", "permissions", ctx);
            throw new Exception(errorMsg);
        }

        private static IContentReference ThrowOnMissingContentReference(this IContentReference? result,
            ISharedResourcesLocalizer localizer, string refTypeName, string refUse, string context)
        {
            if (result is not null)
            {
                return result;
            }

            var errorMsg = BuildMissingContentReferenceError(localizer, refTypeName, refUse, context);
            throw new Exception(errorMsg);
        }

        internal static IContentReference ThrowOnMissingContentReference<TContentRef>(this IContentReference? result,
            ISharedResourcesLocalizer localizer, string refUse, ContentLocation missingSourceLocation)
            => result.ThrowOnMissingContentReference(localizer, MigrationPipelineContentType.GetDisplayNameForType(typeof(TContentRef)), refUse,
                LocationContext(missingSourceLocation));

        internal static IContentReference ThrowOnMissingContentReference(this IContentReference? result,
            ISharedResourcesLocalizer localizer, string contentRefType, string refUse, Guid missingSourceId)
            => result.ThrowOnMissingContentReference(localizer, contentRefType, refUse,
                IdContext(missingSourceId));

        internal static IContentReference ThrowOnMissingContentReference<TContentRef>(this IContentReference? result,
            ISharedResourcesLocalizer localizer, string refUse, Guid missingSourceId)
            => result.ThrowOnMissingContentReference(localizer, MigrationPipelineContentType.GetDisplayNameForType(typeof(TContentRef)), refUse, missingSourceId);

        internal static IContentReference ThrowOnMissingContentReference<TContentRef>(this IContentReference? result,
            ISharedResourcesLocalizer localizer, string refUse, string missingContentUrl)
            => result.ThrowOnMissingContentReference(localizer, MigrationPipelineContentType.GetDisplayNameForType(typeof(TContentRef)), refUse,
                ContentUrlContext(missingContentUrl));
    }
}
