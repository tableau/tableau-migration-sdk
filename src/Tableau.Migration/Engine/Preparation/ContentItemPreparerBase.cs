﻿//
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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// Abstract base class for <see cref="IContentItemPreparer{TContent, TPublish}"/> implementations that defines standard features.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPrepare">The pull return type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public abstract class ContentItemPreparerBase<TContent, TPrepare, TPublish> : IContentItemPreparer<TContent, TPublish>
        where TPrepare : class
        where TPublish : class
    {
        private readonly IContentItemConverter<TPrepare, TPublish> _converter;
        private readonly IContentTransformerRunner _transformerRunner;
        private readonly IDestinationContentReferenceFinderFactory _destinationFinderFactory;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="ContentItemPreparerBase{TContent, TPrepare, TPublish}"/> object.
        /// </summary>
        /// <param name="pipeline">The migration pipeline.</param>
        /// <param name="transformerRunner">A transformer runner.</param>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="localizer">A localizer.</param>
        public ContentItemPreparerBase(IMigrationPipeline pipeline,
            IContentTransformerRunner transformerRunner,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ISharedResourcesLocalizer localizer)
        {
            _converter = pipeline.GetItemConverter<TPrepare, TPublish>();
            _transformerRunner = transformerRunner;
            _destinationFinderFactory = destinationFinderFactory;
            _localizer = localizer;
        }

        /// <summary>
        /// Pulls any additional information needed to prepare a content item for conversion and/or publishing.
        /// </summary>
        /// <param name="item">The content item to use to pull additional information.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The item to use for conversion and/or publishing.</returns>
        protected abstract Task<IResult<TPrepare>> PullAsync(ContentMigrationItem<TContent> item, CancellationToken cancel);

        /// <summary>
        /// Performs pre-publishing modifications on a publish item.
        /// </summary>
        /// <param name="publishItem">The item intended for publishing to prepare.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The item to use for publishing.</returns>
        protected virtual async Task<IResult<TPublish>> TransformAsync(TPublish publishItem, CancellationToken cancel)
        {
            var transformedItem = await _transformerRunner.ExecuteAsync(publishItem, cancel).ConfigureAwait(false);
            return Result<TPublish>.Succeeded(transformedItem);
        }

        /// <summary>
        /// Applies the mapped destination location to the item to publish.
        /// </summary>
        /// <param name="publishItem">The item to publish.</param>
        /// <param name="mappedLocation">The mapped destination location.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the mapping application.</returns>
        protected virtual async Task ApplyMappingAsync(TPublish publishItem, ContentLocation mappedLocation, CancellationToken cancel)
        {
            /* We do interface inspection by default
             * because using generic type resolution
             * would require generic constraints to bubble up to
             * at least the content migrator level.*/

            /* If this logic gets more complicated it 
             * can be moved into a pipeline-resolved service
             * but will still likely be better casting instead of
             * generics.*/

            /* Applying mapping means that content item properties
             * that are involved in mapping (name, parent project, etc.)
             * should be readonly so transformers don't change values that
             * can't be reflected in mapping.*/

            if (publishItem is IMappableContainerContent containerContent)
            {
                /* Item belongs to a mappable container, i.e. projects and personal spaces.
                 * We update the container reference based on the mapped container location so publishing has the right information.
                 * Some content types (projects) this container is optional,
                 * while others (e.g. data sources and workbooks) require a container.*/

                IContentReference? newParent;
                var mappedParentLocation = mappedLocation.Parent();

                if (mappedParentLocation.IsEmpty)
                {
                    //Item is explicitly mapped to the top-level, so should not have a parent container.
                    newParent = null;
                }
                else if (mappedParentLocation != containerContent.Container?.Location)
                {
                    /* Item was pre-mapped for us, but assigned a new parent container.
                     * Find the destination project reference to assign without double-mapping.*/
                    var destinationFinder = _destinationFinderFactory.ForDestinationContentType<IProject>();
                    newParent = await destinationFinder.FindByMappedLocationAsync(mappedLocation.Parent(), cancel)
                        .ConfigureAwait(false);

                    //Without a valid destination container we cannot publish.
                    if (newParent is null)
                    {
                        throw new Exception(string.Format(_localizer[SharedResourceKeys.ContainerParentNotFound], mappedLocation.Parent(), mappedLocation));
                    }
                }
                else if (containerContent.Container is not null)
                {
                    /* Item was mapped without any change to the container path.
                     * We apply project mapping to find where the container was mapped to.*/
                    var destinationFinder = _destinationFinderFactory.ForDestinationContentType<IProject>();
                    newParent = await destinationFinder.FindBySourceLocationAsync(containerContent.Container.Location, cancel)
                        .ConfigureAwait(false);

                    //Without a valid destination container we cannot publish.
                    if (newParent is null)
                    {
                        throw new Exception(string.Format(_localizer[SharedResourceKeys.ContainerParentNotFound], string.Empty, containerContent.Container.Location));
                    }
                }
                else
                {
                    //Item has a null container.
                    newParent = null;
                }

                containerContent.SetLocation(newParent, mappedLocation);
            }

            if (publishItem is IMappableContent mappableContent)
            {
                mappableContent.SetLocation(mappedLocation);
            }
        }

        /// <summary>
        /// Performs finalization tasks that occur after transformers, but before publishing.
        /// </summary>
        /// <param name="publishItem">The item to publish.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await.</returns>
        protected virtual async Task FinalizeAsync(TPublish publishItem, CancellationToken cancel)
        {
            //If the item has a file we close any open editor
            //so that changes are persisted to disk and ready for file upload.
            if (publishItem is IFileContent fileContent)
            {
                await fileContent.File.CloseTableauFileEditorAsync(cancel).ConfigureAwait(false);
            }
        }

        private async Task<IResult<TPublish>> PreparePublishItemAsync(ContentMigrationItem<TContent> item, TPublish publishItem, CancellationToken cancel)
        {
            bool finalizeAttempted = false;

            try
            {
                cancel.ThrowIfCancellationRequested();

                /* Apply the mapped location to the item to publish.
                 * This is done here instead of in a transformer
                 * so that users can't remove (accidentally or otherwise)
                 * a core expectation of publishing logic, namely that mapping
                 * content actually applies any renames/etc.*/
                await ApplyMappingAsync(publishItem, item.ManifestEntry.MappedLocation, cancel)
                    .ConfigureAwait(false);

                cancel.ThrowIfCancellationRequested();

                var transformResult = await TransformAsync(publishItem, cancel).ConfigureAwait(false);

                /* Finalize the transformed item to publish.
                 * This is done here instead of in a transformer
                 * so that we know that all transformers have run,
                 * and publishing hasn't started.*/
                finalizeAttempted = true;
                await FinalizeAsync(publishItem, cancel).ConfigureAwait(false);

                if (!transformResult.Success)
                {
                    item.ManifestEntry.SetFailed(transformResult.Errors);
                    return transformResult;
                }

                publishItem = transformResult.Value;
                return Result<TPublish>.Succeeded(publishItem);
            }
            catch (Exception ex)
            {
                //First try to close any open file editors, 
                //but skip this if finalization is what caused us to throw.
                if (!finalizeAttempted)
                {
                    try
                    {
                        await FinalizeAsync(publishItem, cancel).ConfigureAwait(false);
                    }
                    catch (Exception finalizeEx)
                    {
                        //If finalize throws we still want to attempt to dispose,
                        //but don't want to lose the exception.
                        throw new AggregateException(ex, finalizeEx);
                    }
                }

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IResult<TPublish>> PrepareAsync(ContentMigrationItem<TContent> item, CancellationToken cancel)
        {
            /* Some content types require more information than the source paging provides.
             * The 'pull' step allows for making one or more API calls to fully load the item to publish.
             * By default the pull step just re-uses the source list item.*/
            cancel.ThrowIfCancellationRequested();

            var pullResult = await PullAsync(item, cancel).ConfigureAwait(false);
            if (!pullResult.Success)
            {
                item.ManifestEntry.SetFailed(pullResult.Errors);
                return pullResult.CastFailure<TPublish>();
            }

            var prepareItem = pullResult.Value;

            /* If we throw (even from cancellation) before we can return the publish item,
             * make sure the item is disposed as there may be files using disk space.
             * We clean up orphaned files at the end of the DI scope, but we don't want to 
             * bloat disk usage when we're processing future pages of items.*/
            var prepareResult = await prepareItem.DisposeOnThrowAsync(async () =>
            {
                /* Convert step: Convert the pulled item to a publishable item,
                 * which may be for different endpoint flavors.
                 * For example, a Tableau Server schedule needs conversion to a Tableau Cloud schedule. */
                var publishItem = await _converter.ConvertAsync(prepareItem, cancel).ConfigureAwait(false);

                /* Map and transform the publishable item.
                 * Disposing if any exception prevents our return. */
                return await publishItem.DisposeOnThrowAsync(async () =>
                    await PreparePublishItemAsync(item, publishItem, cancel).ConfigureAwait(false)
                ).ConfigureAwait(false);

            }).ConfigureAwait(false);

            return prepareResult;
        }
    }
}
