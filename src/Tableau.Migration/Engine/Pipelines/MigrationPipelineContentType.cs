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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Object that represents a definition of a content type
    /// that a pipeline migrates.
    /// </summary>
    /// <param name="ContentType">The content type. Content type is returned from list step, pre-pull.</param>
    public record MigrationPipelineContentType(Type ContentType)
    {
        /// <summary>
        /// Gets the user <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Users = new MigrationPipelineContentType<IUser>();

        /// <summary>
        /// Gets the groups <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Groups = new MigrationPipelineContentType<IGroup>()
            .WithPrepareType<IPublishableGroup>()
            .WithPublishType<IPublishableGroup>();

        /// <summary>
        /// Gets the projects <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Projects = new MigrationPipelineContentType<IProject>();

        /// <summary>
        /// Gets the data sources <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType DataSources = new MigrationPipelineContentType<IDataSource>()
            .WithPrepareType<IPublishableDataSource>()
            .WithPublishType<IPublishableDataSource>()
            .WithResultType<IDataSourceDetails>();

        /// <summary>
        /// Gets the workbooks <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Workbooks = new MigrationPipelineContentType<IWorkbook>()
            .WithPrepareType<IPublishableWorkbook>()
            .WithPublishType<IPublishableWorkbook>()
            .WithResultType<IWorkbookDetails>();

        /// <summary>
        /// Gets the views <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Views = new MigrationPipelineContentType<IView>();

        /// <summary>
        /// Gets the Server to Server extract refresh tasks <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType ServerToServerExtractRefreshTasks = new MigrationPipelineContentType<IServerExtractRefreshTask>();

        /// <summary>
        /// Gets the Server to Cloud extract refresh tasks <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType ServerToCloudExtractRefreshTasks = new MigrationPipelineContentType<IServerExtractRefreshTask>()
            .WithPublishType<ICloudExtractRefreshTask>();

        /// <summary>
        /// Gets the Cloud to Cloud extract refresh tasks <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType CloudToCloudExtractRefreshTasks = new MigrationPipelineContentType<ICloudExtractRefreshTask>();

        /// <summary>
        /// Gets the custom views <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType CustomViews = new MigrationPipelineContentType<ICustomView>()
            .WithPrepareType<IPublishableCustomView>()
            .WithPublishType<IPublishableCustomView>();

        /// <summary>
        /// Gets the Server to Server subscriptions <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType ServerToServerSubscriptions = new MigrationPipelineContentType<IServerSubscription>();

        /// <summary>
        /// Gets the Server to Cloud subscriptions <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType ServerToCloudSubscriptions = new MigrationPipelineContentType<IServerSubscription>()
            .WithPublishType<ICloudSubscription>();

        /// <summary>
        /// Gets the Cloud to Cloud subscriptions <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType CloudToCloudSubscriptions = new MigrationPipelineContentType<ICloudSubscription>();

        /// <summary>
        /// Gets the preparation type that is pulled and converted for publishing. The Prepare type is the post-pull, pre-conversion type.
        /// </summary>
        public Type PrepareType { get; private init; } = ContentType;

        /// <summary>
        /// Gets the publish type. The publish type is post-conversion, ready to publish.
        /// </summary>
        public Type PublishType { get; private init; } = ContentType;

        /// <summary>
        /// Gets the result type returned by publishing.
        /// </summary>
        public Type ResultType { get; private init; } = ContentType;

        /// <summary>
        /// Gets the types for this instance.
        /// </summary>
        public IImmutableList<Type> Types => new[] { ContentType, PrepareType, PublishType, ResultType }.Distinct().ToImmutableArray();

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified preparation type. 
        /// Preperation type is post-pull, pre-conversion.
        /// </summary>
        /// <param name="prepareType">The preparation type.</param>
        public MigrationPipelineContentType WithPrepareType(Type prepareType)
            => new(ContentType) { PrepareType = prepareType, PublishType = PublishType, ResultType = ResultType };

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified preparation type.
        /// Preperation type is post-pull, pre-conversion.
        /// </summary>
        public MigrationPipelineContentType WithPrepareType<TPrepare>()
            => WithPrepareType(typeof(TPrepare));

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified publish type.
        /// Publish type is post-conversion, ready to publish.
        /// </summary>
        /// <param name="publishType">The publish type.</param>
        public MigrationPipelineContentType WithPublishType(Type publishType)
            => new(ContentType) { PrepareType = PrepareType, PublishType = publishType, ResultType = ResultType };

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified publish type.
        /// Publish type is post-conversion, ready to publish.
        /// </summary>
        public MigrationPipelineContentType WithPublishType<TPublish>()
            => WithPublishType(typeof(TPublish));

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified result type.
        /// Result type is post-publish.
        /// </summary>
        /// <param name="resultType">The result type.</param>
        public MigrationPipelineContentType WithResultType(Type resultType)
            => new(ContentType) { PrepareType = PrepareType, PublishType = PublishType, ResultType = resultType };

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified result type.
        /// Result type is post-publish.
        /// </summary>
        public MigrationPipelineContentType WithResultType<TResult>()
            => WithResultType(typeof(TResult));

        /// <summary>
        /// Gets the <see cref="PublishType"/> value if it implements the given interface, or null if it does not.
        /// </summary>
        /// <param name="interface">The interface to search for.</param>
        public Type[]? GetPublishTypeForInterface(Type @interface)
            => HasInterface(PublishType, @interface) ? [PublishType] : null;

        /// <summary>
        /// Gets the <see cref="ContentType"/> value if it implements the given interface, or null if it does not.
        /// </summary>
        /// <param name="interface">The interface to search for.</param>
        public Type[]? GetContentTypeForInterface(Type @interface)
            => HasInterface(ContentType, @interface) ? [ContentType] : null;

        /// <summary>
        /// Gets the <see cref="PublishType"/> and <see cref="ResultType"/> array if it implements the given interface, or null if it does not.
        /// </summary>
        /// <param name="interface">The interface to search for.</param>
        public Type[]? GetPostPublishTypesForInterface(Type @interface)
            => HasInterface(PublishType, @interface) ? [PublishType, ResultType] : null;

        /// <summary>
        /// Gets the config key for this content type.
        /// </summary>
        /// <returns>The config key string.</returns>
        public string GetConfigKey()
             => GetConfigKeyForType(ContentType);

        /// <summary>
        /// Gets the config key for a content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <returns>The config key string.</returns>
        public static string GetConfigKeyForType(Type contentType)
        {
            if (!contentType.IsGenericType)
            {
                var typeName = contentType.Name;
                return typeName.TrimStart('I');
            }

            var convertedName = new StringBuilder()
                .Append(
                    contentType.Name
                        .TrimStart('I')
                        .TrimEnd('1')
                        .TrimEnd('`'));

            foreach (var arg in contentType.GenericTypeArguments)
            {
                convertedName.Append($"_{arg.Name.TrimStart('I')}");
            }

            return convertedName.ToString();
        }

        /// <summary>
        /// Gets the friendly display name for a content type.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="plural">Whether the display name should be in plural form.</param>
        /// <returns>The display name string.</returns>
        public static string GetDisplayNameForType(Type contentType, bool plural = false)
        {
            var configKey = GetConfigKeyForType(contentType);

            if (configKey.Length < 2)
            {
                return configKey;
            }

            var sb = new StringBuilder();
            for (var i = 0; i < configKey.Length; i++)
            {
                if (i != 0 && char.IsUpper(configKey[i]))
                {
                    sb.Append(' ');
                }

                sb.Append(configKey[i]);
            }

            if (plural)
            {
                sb.Append('s');
            }

            return sb.ToString();
        }

        private static bool HasInterface(Type t, Type @interface)
            => t.GetInterfaces().Contains(@interface);

        /// <summary>
        /// Gets the content types for a given profile.
        /// </summary>
        /// <param name="profile">Profile to get the types for.</param>
        /// <returns>Array of content types supported by the given pipeline profile.</returns>
        public static ImmutableArray<MigrationPipelineContentType> GetMigrationPipelineContentTypes(PipelineProfile profile)
        {
            switch (profile)
            {
                case PipelineProfile.ServerToServer:
                    return ServerToServerMigrationPipeline.ContentTypes;

                case PipelineProfile.ServerToCloud:
                    return ServerToCloudMigrationPipeline.ContentTypes;

                case PipelineProfile.CloudToCloud:
                    return CloudToCloudMigrationPipeline.ContentTypes;

                default:
                    throw new ArgumentException($"Cannot get content types for profile {profile}");
            }
        }

        /// <summary>
        /// Gets all static instances of <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static IEnumerable<MigrationPipelineContentType> GetAllMigrationPipelineContentTypes()
        {
            return typeof(MigrationPipelineContentType)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(MigrationPipelineContentType))
                .Select(field => (MigrationPipelineContentType?)field.GetValue(null))
                .Where(instance => instance != null)
                .Select(instance => instance!);
        }
    }

    /// <summary>
    /// Object that represents a definition of a content type
    /// that a pipeline migrates.
    /// </summary>
    /// <typeparam name="TContent">The content, publish, result, and list type.</typeparam>
    public sealed record MigrationPipelineContentType<TContent>()
        : MigrationPipelineContentType(typeof(TContent))
    { }
}
