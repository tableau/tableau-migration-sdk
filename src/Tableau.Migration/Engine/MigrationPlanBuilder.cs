// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content;
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
            SetPipelineProfile(PipelineProfile.ServerToCloud);

            _serverToCloudBuilder ??= new(_localizer, this);

            ClearExtensions();

            return _serverToCloudBuilder
                .AppendDefaultServerToCloudExtensions();
        }

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

            // Post-publish hooks.
            Hooks.Add(typeof(OwnerItemPostPublishHook<,>), GetPostPublishTypesByInterface<IRequiresOwnerUpdate>());
            Hooks.Add(typeof(PermissionsItemPostPublishHook<,>), GetPostPublishTypesByInterface<IPermissionsContent>());
            Hooks.Add(typeof(ChildItemsPermissionsPostPublishHook<,>), GetPostPublishTypesByInterface<IChildPermissionsContent>());
            Hooks.Add(typeof(TagItemPostPublishHook<,>), GetPostPublishTypesByInterface<IWithTags>());
            Hooks.Add<ProjectPostPublishHook>();

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
                _simulatorFactory.GetOrCreate(serverUrl);
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
                _simulatorFactory.GetOrCreate(podUrl);
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

        private void SetPipelineProfile(PipelineProfile pipelineProfile)
        {
            _pipelineProfile = pipelineProfile;
            _supportedContentTypes = _pipelineProfile.GetSupportedContentTypes();
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

            var listContentTypes = _supportedContentTypes.Select(x => x.ContentType).ToImmutableHashSet();
            var publishContentTypes = _supportedContentTypes.Select(x => x.PublishType).ToImmutableHashSet();

            foreach (var filterType in Filters.ByContentType())
            {
                if (!listContentTypes.Contains(filterType.Key))
                {
                    errors.Add(new(_localizer[SharedResourceKeys.UnknownFilterContentTypeValidationMessage,
                        filterType.Key.Name, filterType.Value.Count()]));
                }
            }

            foreach (var mappingType in Mappings.ByContentType())
            {
                if (!listContentTypes.Contains(mappingType.Key))
                {
                    errors.Add(new(_localizer[SharedResourceKeys.UnknownMappingContentTypeValidationMessage,
                        mappingType.Key.Name, mappingType.Value.Count()]));
                }
            }

            foreach (var transformerType in Transformers.ByContentType())
            {
                if (!publishContentTypes.Contains(transformerType.Key))
                {
                    //If the user gave us a list content type instead of a publish type
                    //give the user a validation error with a hint to the right type.
                    string errorMessage;
                    if (listContentTypes.Contains(transformerType.Key))
                    {
                        var hintType = _supportedContentTypes.First(x => x.ContentType == transformerType.Key).PublishType;

                        errorMessage = _localizer[SharedResourceKeys.UnknownMappingContentTypeValidationMessage,
                            transformerType.Key.Name, hintType.Name, transformerType.Value.Count()];
                    }
                    else
                    {
                        errorMessage = _localizer[SharedResourceKeys.UnknownMappingContentTypeValidationMessage,
                            transformerType.Key.Name, transformerType.Value.Count()];
                    }

                    errors.Add(new(errorMessage));
                }
            }

            return Result.FromErrors(errors);
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
        public IMigrationPlan Build() =>
            new MigrationPlan(PlanId: Guid.NewGuid(),
                              PipelineProfile: _pipelineProfile,
                              Options: Options.Build(),
                              Source: _source,
                              Destination: _destination,
                              Hooks: Hooks.Build(),
                              Mappings: Mappings.Build(),
                              Filters: Filters.Build(),
                              Transformers: Transformers.Build());
    }
}
