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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default.Cascade
{
    /// <summary>
    /// Abstract base class for a filter that can cascade filters
    /// from any referenced content items that are necessary to migrate a given content item.
    /// </summary>
    public abstract class CascadingFilterBase<TContent> : ContentFilterBase<TContent>
        where TContent : IContentReference
    {
        private readonly IMigrationManifestEditor _manifest;

        /// <summary>
        /// Creates a new <see cref="CascadingFilterBase{TContent}"/> object.
        /// </summary>
        /// <param name="manifest">The manifest to find cascade filter status from.</param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc />.</param>
        protected CascadingFilterBase(IMigrationManifestEditor manifest,
            ISharedResourcesLocalizer? localizer, ILogger<CascadingFilterBase<TContent>>? logger) 
            : base(localizer, logger)
        {
            _manifest = manifest;
        }

        #region - Cascading Reference Evaluation -

        /// <summary>
        /// Finds if a required content reference has been excluded by a cascading filter.
        /// </summary>
        /// <typeparam name="TReference">The reference's content type.</typeparam>
        /// <param name="reference">The reference's value.</param>
        /// <returns>True if the reference has a cascading filter, otherwise false.</returns>
        protected bool HasCascadingFilterReference<TReference>(IContentReference? reference)
            => HasCascadingFilterReference(typeof(TReference), reference);

        /// <summary>
        /// Finds if a required content reference has been excluded by a cascading filter.
        /// </summary>
        /// <typeparam name="TReference">The reference's content type.</typeparam>
        /// <param name="location">The reference's location.</param>
        /// <returns>True if the reference has a cascading filter, otherwise false.</returns>
        protected bool HasCascadingFilterReference<TReference>(ContentLocation? location)
            => HasCascadingFilterReference(typeof(TReference), location);

        /// <summary>
        /// Finds if a required content reference has been excluded by a cascading filter.
        /// </summary>
        /// <param name="referenceType">The reference's content type.</param>
        /// <param name="reference">The reference's value.</param>
        /// <returns>True if the reference has a cascading filter, otherwise false.</returns>
        protected bool HasCascadingFilterReference(Type referenceType, IContentReference? reference)
            => HasCascadingFilterReference(referenceType, reference?.Location);

        /// <summary>
        /// Finds if a required content reference has been excluded by a cascading filter.
        /// </summary>
        /// <param name="referenceType">The reference's content type.</param>
        /// <param name="location">The reference's location.</param>
        /// <returns>True if the reference has a cascading filter, otherwise false.</returns>
        protected bool HasCascadingFilterReference(Type referenceType, ContentLocation? location)
        {
            if (location is null)
            {
                return false;
            }

            var manifestEntries = _manifest.Entries.GetPartition(referenceType);
            if (manifestEntries is null)
            {
                // The reference type isn't tracked in the manifest, so can't have a cascading filter.
                return false;
            }

            if (manifestEntries.BySourceLocation.TryGetValue(location.Value, out var referenceEntry))
            {
                return referenceEntry.CascadeSkip is true;
            }

            return false;
        }

        #endregion

        #region - Interface Inference References -

        private bool HasCascadingFilterContainer(ContentFilterContextItem<TContent> item)
        {
            if(item.SourceItem is IContainerContent containerItem)
            {
                return HasCascadingFilterReference<IProject>(containerItem.Container);
            }
            else if(item.SourceItem is IMappableContainerContent mappableContainerItem)
            {
                return HasCascadingFilterReference<IProject>(mappableContainerItem.Container);
            }

            return false;
        }

        private bool HasCascadingFilterOwner(ContentFilterContextItem<TContent> item)
        {
            if(item.SourceItem is IWithOwner ownerItem)
            {
                return HasCascadingFilterReference<IUser>(ownerItem.Owner);
            }

            return false;
        }

        private bool HasCascadingFilterWorkbook(ContentFilterContextItem<TContent> item)
        {
            if (item.SourceItem is IWithWorkbook workbookItem)
            {
                return HasCascadingFilterReference<IWorkbook>(workbookItem.Workbook);
            }

            return false;
        }

        private bool HasStandardCascadingFilterReferences(ContentFilterContextItem<TContent> item)
        {
            return HasCascadingFilterContainer(item) ||
                HasCascadingFilterOwner(item) ||
                HasCascadingFilterWorkbook(item);
        }

        #endregion

        #region - Filter Logic -

        /// <summary>
        /// Finds if any non-interface driven references have been excluded with a cascading filter.
        /// </summary>
        /// <param name="item">The current item to find content references for.</param>
        /// <returns>True if any content reference has a cascading filter, otherwise false.</returns>
        protected abstract bool HasExtraCascadingFilterReferences(ContentFilterContextItem<TContent> item);

        /// <summary>
        /// Finds if any of the required content references on the current item
        /// have been excluded with a cascading filter.
        /// 
        /// Optional references like many-to-many references are not considered.
        /// </summary>
        /// <param name="item">The current item to find content references for.</param>
        /// <returns>True if any content reference has a cascading filter, otherwise false.</returns>
        protected virtual bool HasCascadingFilterReferences(ContentFilterContextItem<TContent> item)
        {
            if(HasStandardCascadingFilterReferences(item))
            {
                return true;
            }

            return HasExtraCascadingFilterReferences(item);
        }

        /// <inheritdoc />
        public override void Filter(ContentFilterContextItem<TContent> item)
        {
            /*
             * We may potentially "upgrade" previous filter work from skip -> cascade skip,
             * but don't need to run if we're already set to cascade.
             */
            if(item.Status is FilterStatus.CascadeSkip)
            {
                return;
            }

            // Referencing items with cascading filters is transitive.
            if(HasCascadingFilterReferences(item))
            {
                item.Status = FilterStatus.CascadeSkip;
            }
        }

        #endregion
    }
}
