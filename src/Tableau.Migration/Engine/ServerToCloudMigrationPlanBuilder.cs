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
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Services;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Default <see cref="IServerToCloudMigrationPlanBuilder"/> implementation.
    /// </summary>
    public class ServerToCloudMigrationPlanBuilder : IServerToCloudMigrationPlanBuilder
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMigrationPlanBuilder _innerBuilder;

        private bool _authTypeMappingAdded;
        private bool _usernameMappingAdded;

        /// <summary>
        /// Creates a new <see cref="ServerToCloudMigrationPlanBuilder"/> object.
        /// </summary>
        /// <param name="localizer">The string localizer.</param>
        /// <param name="loggerFactory">The logger factory to create new logger types.</param>
        /// <param name="innerBuilder">A general plan builder to wrap.</param>
        public ServerToCloudMigrationPlanBuilder(
            ISharedResourcesLocalizer localizer,
            ILoggerFactory loggerFactory,
            IMigrationPlanBuilder innerBuilder)
        {
            _localizer = localizer;
            _loggerFactory = loggerFactory;
            _innerBuilder = innerBuilder;
        }

        #region - Fluent API Wrapper -

        IMigrationPlanEndpointBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Source => _innerBuilder.Source;

        IMigrationPlanEndpointBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Destination => _innerBuilder.Destination;

        IMigrationPlanOptionsBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Options => _innerBuilder.Options;

        IMigrationServiceBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Services => _innerBuilder.Services;

        IMigrationHookBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Hooks => _innerBuilder.Hooks;

        IContentMappingBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Mappings => _innerBuilder.Mappings;

        IContentFilterBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Filters => _innerBuilder.Filters;

        IContentTransformerBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Transformers => _innerBuilder.Transformers;

        PipelineProfile IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.PipelineProfile => _innerBuilder.PipelineProfile;

        IMigrationPlan IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Build()
            => _innerBuilder.Build();

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.ClearExtensions()
        {
            _innerBuilder.ClearExtensions();
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.AppendDefaultExtensions()
        {
            _innerBuilder.AppendDefaultExtensions();
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.ForServerToCloud()
            => this;

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.ForCustomPipelineFactory(Func<IServiceProvider, IMigrationPipelineFactory> pipelineFactoryOverride, params IEnumerable<MigrationPipelineContentType> supportedContentTypes)
        {
            _innerBuilder.ForCustomPipelineFactory(pipelineFactoryOverride, supportedContentTypes);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.ForCustomPipelineFactory<T>(params IEnumerable<MigrationPipelineContentType> supportedContentTypes)
        {
            _innerBuilder.ForCustomPipelineFactory<T>(supportedContentTypes);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.ForCustomPipeline<T>(params IEnumerable<MigrationPipelineContentType> supportedContentTypes)
        {
            _innerBuilder.ForCustomPipeline<T>(supportedContentTypes);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.FromSource(IMigrationPlanEndpointConfiguration config)
        {
            _innerBuilder.FromSource(config);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.FromSourceTableauServer(Uri serverUrl, string siteContentUrl, string accessTokenName, string accessToken, bool createApiSimulator, string? restApiVersion)
        {
            _innerBuilder.FromSourceTableauServer(serverUrl, siteContentUrl, accessTokenName, accessToken, createApiSimulator, restApiVersion);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.ToDestination(IMigrationPlanEndpointConfiguration config)
        {
            _innerBuilder.ToDestination(config);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.ToDestinationTableauCloud(Uri podUrl, string siteContentUrl, string accessTokenName, string accessToken, bool createApiSimulator, string? restApiVersion)
        {
            _innerBuilder.ToDestinationTableauCloud(podUrl, siteContentUrl, accessTokenName, accessToken, createApiSimulator, restApiVersion);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.SkipContentType<TContent>(bool preCache)
        {
            _innerBuilder.SkipContentType<TContent>(preCache);
            return this;
        }

        IServerToCloudMigrationPlanBuilder IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.SkipContentType(Type contentType, bool preCache)
        {
            _innerBuilder.SkipContentType(contentType, preCache);
            return this;
        }

        IResult IMigrationPlanBuilder<IServerToCloudMigrationPlanBuilder>.Validate()
            => _innerBuilder.Validate();

        #endregion

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder AppendDefaultServerToCloudExtensions()
        {
            _innerBuilder.AppendDefaultExtensions();

            //Add standard server-to-cloud hooks, filters, etc.
            //specific to server to cloud migrations here.

            //Default server-to-cloud filters.
            _innerBuilder.Filters.Add<UserSiteRoleSupportUserFilter, IUser>();

            //Default server-to-cloud transformers            
            _innerBuilder.Transformers.Add<UserTableauCloudSiteRoleTransformer, IUser>();

            return this;
        }

        #region - WithAuthenticationType -

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithSamlAuthenticationType(string domain, string? idpConfigurationName = null)
            => WithAuthenticationType(idpConfigurationName ?? AuthenticationTypes.Saml, domain, Constants.LocalDomain);

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithTableauIdAuthenticationType(bool mfa = true, string? idpConfigurationName = null)
        {
            if (mfa)
            {
                return WithAuthenticationType(idpConfigurationName ?? AuthenticationTypes.TableauIdWithMfa, Constants.TableauIdWithMfaDomain, Constants.LocalDomain);
            }
            else
            {
                return WithAuthenticationType(idpConfigurationName ?? AuthenticationTypes.OpenId, Constants.ExternalDomain, Constants.LocalDomain);
            }
        }

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithAuthenticationType(string authenticationType, string userDomain, string groupDomain)
            => WithAuthenticationType(authenticationType, () =>
            {
                _innerBuilder.Mappings.Add<AuthenticationTypeDomainMapping, IUser>();
                _innerBuilder.Mappings.Add<AuthenticationTypeDomainMapping, IGroup>();
                _innerBuilder.Options.Configure(new AuthenticationTypeDomainMappingOptions
                {
                    UserDomain = userDomain,
                    GroupDomain = groupDomain
                });
            });

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithAuthenticationType(string authenticationType, IAuthenticationTypeDomainMapping authenticationTypeMapping)
            => WithAuthenticationType(authenticationType, () =>
            {
                _innerBuilder.Mappings.Add<IUser>(authenticationTypeMapping);
                _innerBuilder.Mappings.Add<IGroup>(authenticationTypeMapping);
            });

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithAuthenticationType<TMapping>(string authenticationType, Func<IServiceProvider, TMapping>? authenticationTypeMappingFactory = null)
            where TMapping : IAuthenticationTypeDomainMapping
            => WithAuthenticationType(authenticationType, () =>
            {
                _innerBuilder.Mappings.Add<TMapping, IUser>(authenticationTypeMappingFactory);
                _innerBuilder.Mappings.Add<TMapping, IGroup>(authenticationTypeMappingFactory);
            });

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithAuthenticationType(string authenticationType, Func<ContentMappingContext<IUsernameContent>, CancellationToken, Task<string?>> callback)
            => WithAuthenticationType(authenticationType, new CallbackAuthenticationTypeDomainMapping(callback, _localizer, _loggerFactory.CreateLogger<CallbackAuthenticationTypeDomainMapping>()));

        private ServerToCloudMigrationPlanBuilder WithAuthenticationType(string authenticationType, Action registerMappings)
        {
            //Register a default mapper for user/group domains based on the authentication type.
            registerMappings();

            //Configure the default registered auth type transformer to match the user-supplied auth type.
            _innerBuilder.Options.Configure(new UserAuthenticationTypeTransformerOptions
            {
                AuthenticationType = authenticationType
            });

            _authTypeMappingAdded = true;
            return this;
        }

        #endregion

        #region - WithTableauCloudUsernames -

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames(string mailDomain, bool useExistingEmail = true)
        {
            _innerBuilder.Mappings.Add<TableauCloudUsernameMapping, IUser>();
            _innerBuilder.Options.Configure(new TableauCloudUsernameMappingOptions
            {
                MailDomain = mailDomain,
                UseExistingEmail = useExistingEmail
            });

            _usernameMappingAdded = true;
            return this;
        }

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames(ITableauCloudUsernameMapping usernameMapping)
        {
            _innerBuilder.Mappings.Add(usernameMapping);

            _usernameMappingAdded = true;
            return this;
        }

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames<TMapping>(Func<IServiceProvider, TMapping>? usernameMappingFactory = null)
            where TMapping : ITableauCloudUsernameMapping
        {
            _innerBuilder.Mappings.Add<TMapping, IUser>(usernameMappingFactory);

            _usernameMappingAdded = true;
            return this;
        }

        /// <inheritdoc />
        public IServerToCloudMigrationPlanBuilder WithTableauCloudUsernames(
            Func<ContentMappingContext<IUser>, CancellationToken, Task<ContentMappingContext<IUser>?>> callback)
        {
            _innerBuilder.Mappings.Add(callback);

            _usernameMappingAdded = true;
            return this;
        }

        #endregion

        /// <summary>
        /// Validates the plan's server-to-cloud specific validation rules.
        /// </summary>
        /// <returns>The validation result.</returns>
        public IResult ValidateServerToCloud()
        {
            var errors = new List<ValidationException>();

            if (!_authTypeMappingAdded)
            {
                errors.Add(new(_localizer[SharedResourceKeys.AuthenticationTypeDomainMappingValidationMessage]));
            }

            if (!_usernameMappingAdded)
            {
                errors.Add(new(_localizer[SharedResourceKeys.TableauCloudUsernameMappingValidationMessage]));
            }

            return Result.FromErrors(errors);
        }
    }
}
