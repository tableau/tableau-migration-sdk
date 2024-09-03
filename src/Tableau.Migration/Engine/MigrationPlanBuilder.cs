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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Default <see cref="IMigrationPlanBuilder"/> implementation.
    /// </summary>
    public class MigrationPlanBuilder : IMigrationPlanBuilder
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ITableauApiSimulatorFactory _simulatorFactory;

        private Func<IServiceProvider, IMigrationPipelineFactory>? _pipelineFactoryOverride;
        private ServerToCloudMigrationPlanBuilder? _serverToCloudBuilder;

        private PipelineProfile _pipelineProfile;
        private IMigrationPlanEndpointConfiguration _source;
        private IMigrationPlanEndpointConfiguration _destination;
        private IImmutableList<MigrationPipelineContentType> _supportedContentTypes = ImmutableArray<MigrationPipelineContentType>.Empty;

        /// <summary>
        /// Creates a new <see cref="MigrationPlanBuilder"/> object.
        /// </summary>
        /// <param name="localizer">The string localizer.</param>
        /// <param name="simulatorFactory">A simulator factory.</param>
        /// <param name="options">A new/fresh options builder.</param>
        /// <param name="hooks">A new/fresh hook builder.</param>
        /// <param name="mappings">A new/fresh mapping builder.</param>
        /// <param name="filters">A new/fresh filter builder.</param>
        /// <param name="transformers">A new/fresh transformer builder.</param>
        public MigrationPlanBuilder(
            ISharedResourcesLocalizer localizer,
            ITableauApiSimulatorFactory simulatorFactory,
            IMigrationPlanOptionsBuilder options,
            IMigrationHookBuilder hooks,
            IContentMappingBuilder mappings,
            IContentFilterBuilder filters,
            IContentTransformerBuilder transformers)
        {
            _localizer = localizer;
            _simulatorFactory = simulatorFactory;

            _source = TableauApiEndpointConfiguration.Empty;
            _destination = TableauApiEndpointConfiguration.Empty;
            Options = options;
            Hooks = hooks;
            Mappings = mappings;
            Filters = filters;
            Transformers = transformers;
        }

        /// <inheritdoc />
        public IMigrationPlanBuilder ClearExtensions()
        {
            Hooks.Clear();
            Mappings.Clear();
            Filters.Clear();
            Transformers.Clear();

            return this;
        }

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder ForServerToCloud()
        {
            SetPipelineProfile(PipelineProfile.ServerToCloud, ServerToCloudMigrationPipeline.ContentTypes);

            _pipelineFactoryOverride = null;
            _serverToCloudBuilder ??= new(_localizer, this);

            ClearExtensions();

            return _serverToCloudBuilder
                .AppendDefaultServerToCloudExtensions();
        }

        /// <inheritdoc />
        public IMigrationPlanBuilder ForCustomPipelineFactory(Func<IServiceProvider, IMigrationPipelineFactory> pipelineFactoryOverride, params MigrationPipelineContentType[] supportedContentTypes)
            => ForCustomPipelineFactory(pipelineFactoryOverride, (IEnumerable<MigrationPipelineContentType>)supportedContentTypes);

        /// <inheritdoc />
        public IMigrationPlanBuilder ForCustomPipelineFactory(Func<IServiceProvider, IMigrationPipelineFactory> pipelineFactoryOverride, IEnumerable<MigrationPipelineContentType> supportedContentTypes)
        {
            SetPipelineProfile(PipelineProfile.Custom, supportedContentTypes);

            _pipelineFactoryOverride = pipelineFactoryOverride;
            _serverToCloudBuilder = null;

            ClearExtensions();
            AppendDefaultExtensions();

            return this;
        }

        /// <inheritdoc />
        public IMigrationPlanBuilder ForCustomPipelineFactory<T>(params MigrationPipelineContentType[] supportedContentTypes)
            where T : IMigrationPipelineFactory
            => ForCustomPipelineFactory<T>((IEnumerable<MigrationPipelineContentType>)supportedContentTypes);

        /// <inheritdoc />
        public IMigrationPlanBuilder ForCustomPipelineFactory<T>(IEnumerable<MigrationPipelineContentType> supportedContentTypes)
            where T : IMigrationPipelineFactory
            => ForCustomPipelineFactory(s => s.GetRequiredService<T>(), supportedContentTypes);

        /// <inheritdoc />
        public IMigrationPlanBuilder ForCustomPipeline<T>(params MigrationPipelineContentType[] supportedContentTypes)
            where T : IMigrationPipeline
            => ForCustomPipeline<T>((IEnumerable<MigrationPipelineContentType>)supportedContentTypes);

        /// <inheritdoc />
        public IMigrationPlanBuilder ForCustomPipeline<T>(IEnumerable<MigrationPipelineContentType> supportedContentTypes)
            where T : IMigrationPipeline
            => ForCustomPipelineFactory<CustomMigrationPipelineFactory<T>>(supportedContentTypes);

        /// <inheritdoc />
        public IMigrationPlanBuilder AppendDefaultExtensions()
        {
            //Add standard hooks, filters, etc. for all migrations here.

            //Standard migration filters.
            Filters.Add(typeof(PreviouslyMigratedFilter<>), GetAllContentTypes());
            Filters.Add<GroupAllUsersFilter, IGroup>();
            Filters.Add(typeof(SystemOwnershipFilter<>), GetContentTypesByInterface<IWithOwner>());

            //Standard migration transformers.
            Transformers.Add<UserAuthenticationTypeTransformer, IUser>();
            Transformers.Add<GroupUsersTransformer, IPublishableGroup>();
            Transformers.Add(typeof(OwnershipTransformer<>), GetPublishTypesByInterface<IWithOwner>());
            Transformers.Add<TableauServerConnectionUrlTransformer, IPublishableWorkbook>();
            Transformers.Add<MappedReferenceExtractRefreshTaskTransformer, ICloudExtractRefreshTask>();
            Transformers.Add<CloudIncrementalRefreshTransformer, ICloudExtractRefreshTask>();
            Transformers.Add(typeof(CloudScheduleCompatibilityTransformer<>), GetPublishTypesByInterface<IWithSchedule<ICloudSchedule>>());
            Transformers.Add(typeof(WorkbookReferenceTransformer<>), GetPublishTypesByInterface<IWithWorkbook>());
            Transformers.Add<CustomViewDefaultUserReferencesTransformer, IPublishableCustomView>();
            Transformers.Add(typeof(EncryptExtractTransformer<>), GetPublishTypesByInterface<IExtractContent>());

            // Post-publish hooks.
            Hooks.Add(typeof(OwnerItemPostPublishHook<,>), GetPostPublishTypesByInterface<IRequiresOwnerUpdate>());
            Hooks.Add(typeof(PermissionsItemPostPublishHook<,>), GetPostPublishTypesByInterface<IPermissionsContent>());
            Hooks.Add(typeof(ChildItemsPermissionsPostPublishHook<,>), GetPostPublishTypesByInterface<IChildPermissionsContent>());
            Hooks.Add(typeof(TagItemPostPublishHook<,>), GetPostPublishTypesByInterface<IWithTags>());
            Hooks.Add<ProjectPostPublishHook>();
            Hooks.Add<CustomViewDefaultUsersPostPublishHook>();

            return this;
        }

        /// <inheritdoc />
        public IMigrationPlanBuilder FromSource(IMigrationPlanEndpointConfiguration config)
        {
            _source = config;
            return this;
        }

        /// <inheritdoc />
        public IMigrationPlanBuilder FromSourceTableauServer(Uri serverUrl, string siteContentUrl, string accessTokenName, string accessToken, bool createApiSimulator = false)
        {
            if (createApiSimulator)
            {
                _simulatorFactory.GetOrCreate(serverUrl, true);
            }

            return FromSource(new TableauApiEndpointConfiguration(new(serverUrl, siteContentUrl, accessTokenName, accessToken)));
        }

        /// <inheritdoc />
        public IMigrationPlanBuilder ToDestination(IMigrationPlanEndpointConfiguration config)
        {
            _destination = config;
            return this;
        }

        /// <inheritdoc />
        public IMigrationPlanBuilder ToDestinationTableauCloud(Uri podUrl, string siteContentUrl, string accessTokenName, string accessToken, bool createApiSimulator = false)
        {
            if (createApiSimulator)
            {
                _simulatorFactory.GetOrCreate(podUrl, false);
            }

            return ToDestination(new TableauApiEndpointConfiguration(new(podUrl, siteContentUrl, accessTokenName, accessToken)));
        }

        /// <inheritdoc />
        public IMigrationPlanOptionsBuilder Options { get; }

        /// <inheritdoc />
        public IMigrationHookBuilder Hooks { get; }

        /// <inheritdoc />
        public IContentMappingBuilder Mappings { get; }

        /// <inheritdoc />
        public IContentFilterBuilder Filters { get; }

        /// <inheritdoc />
        public IContentTransformerBuilder Transformers { get; }

        private void SetPipelineProfile(PipelineProfile pipelineProfile, IEnumerable<MigrationPipelineContentType> supportedContentTypes)
        {
            _pipelineProfile = pipelineProfile;
            _supportedContentTypes = supportedContentTypes.ToImmutableArray();
        }

        private IImmutableList<Type[]> GetAllContentTypes()
            => _supportedContentTypes.Select(t => new[] { t.ContentType }).ToImmutableArray();

        private IImmutableList<Type[]> GetContentTypesByInterface<TInterface>()
            => _supportedContentTypes.WithContentTypeInterface<TInterface>();

        private IImmutableList<Type[]> GetPostPublishTypesByInterface<TInterface>()
            => _supportedContentTypes.WithPostPublishTypeInterface<TInterface>();

        private IImmutableList<Type[]> GetPublishTypesByInterface<TInterface>()
            => _supportedContentTypes.WithPublishTypeInterface<TInterface>();

        private IResult ValidateHookContentTypes()
        {
            //If the user has not set a pipeline profile we
            //validate that elsewhere.
            if (_pipelineProfile == default)
            {
                return Result.Succeeded();
            }

            var errors = new List<ValidationException>();
            var listContentTypes = GetListContentTypes();
            var publishContentTypes = GetPublishContentTypes();

            errors.AddRange(ValidateFilterContentTypes(listContentTypes));
            errors.AddRange(ValidateMappingContentTypes(listContentTypes));
            errors.AddRange(ValidateTransformerContentTypes(listContentTypes, publishContentTypes));

            return Result.FromErrors(errors);
        }

        internal ImmutableHashSet<Type> GetListContentTypes()
            => _supportedContentTypes.Select(x => x.ContentType).ToImmutableHashSet();

        internal ImmutableHashSet<Type> GetPublishContentTypes()
            => _supportedContentTypes.Select(x => x.PublishType).ToImmutableHashSet();

        internal List<ValidationException> ValidateFilterContentTypes(ImmutableHashSet<Type> listContentTypes)
        {
            var errors = new List<ValidationException>();

            foreach (var filterType in Filters.ByContentType())
            {
                if (listContentTypes.Contains(filterType.Key))
                {
                    continue;
                }

                errors.Add(new(_localizer[SharedResourceKeys.UnknownFilterContentTypeValidationMessage,
                    filterType.Key.Name, filterType.Value.Count()]));
            }
            return errors;
        }

        internal List<ValidationException> ValidateMappingContentTypes(ImmutableHashSet<Type> listContentTypes)
        {
            var errors = new List<ValidationException>();

            foreach (var mappingType in Mappings.ByContentType())
            {
                if (listContentTypes.Contains(mappingType.Key))
                {
                    continue;
                }

                errors.Add(new(_localizer[SharedResourceKeys.UnknownMappingContentTypeValidationMessage,
                    mappingType.Key.Name, mappingType.Value.Count()]));
            }
            return errors;
        }

        internal List<ValidationException> ValidateTransformerContentTypes(
            ImmutableHashSet<Type> listContentTypes,
            ImmutableHashSet<Type> publishContentTypes)
        {
            var errors = new List<ValidationException>();

            foreach (var transformerType in Transformers.ByContentType())
            {
                if (publishContentTypes.Contains(transformerType.Key))
                {
                    continue;
                }

                //If the user gave us a list content type instead of a publish type
                //give the user a validation error with a hint to the right type.                
                if (listContentTypes.Contains(transformerType.Key))
                {
                    var hintType = _supportedContentTypes.First(x
                        => x.ContentType == transformerType.Key)
                        .PublishType;

                    errors.Add(new(_localizer[
                        SharedResourceKeys.UnknownTransformerContentTypeValidationMessage,
                        transformerType.Key.Name,
                        hintType.Name,
                        transformerType.Value.Count()]));

                    continue;
                }

                errors.Add(new(_localizer[
                    SharedResourceKeys.UnknownTransformerContentTypeValidationMessage,
                    transformerType.Key.Name,
                    transformerType.Value.Count()]));
            }

            return errors;
        }

        /// <inheritdoc />
        public IResult Validate()
        {
            var resultBuilder = new ResultBuilder();

            var migrationPlan = Build();

            resultBuilder.Add(migrationPlan.ValidateSimpleProperties());
            resultBuilder.Add(migrationPlan.Source.Validate());
            resultBuilder.Add(migrationPlan.Destination.Validate());

            if (_serverToCloudBuilder is not null)
            {
                resultBuilder.Add(_serverToCloudBuilder.ValidateServerToCloud());
            }

            resultBuilder.Add(ValidateHookContentTypes());

            return resultBuilder.Build();
        }

        /// <inheritdoc />
        public IMigrationPlan Build()
            => new MigrationPlan(
                PlanId: Guid.NewGuid(),
                PipelineProfile: _pipelineProfile,
                Options: Options.Build(),
                Source: _source,
                Destination: _destination,
                Hooks: Hooks.Build(),
                Mappings: Mappings.Build(),
                Filters: Filters.Build(),
                Transformers: Transformers.Build(),
                PipelineFactoryOverride: _pipelineFactoryOverride);
    }
}
