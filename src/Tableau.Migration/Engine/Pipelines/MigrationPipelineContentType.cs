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
using System.Collections.Immutable;
using System.Linq;
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
    /// <param name="ContentType">The content type.</param>
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
            .WithPublishType<IPublishableGroup>();

        /// <summary>
        /// Gets the projects <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Projects = new MigrationPipelineContentType<IProject>();

        /// <summary>
        /// Gets the data sources <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType DataSources = new MigrationPipelineContentType<IDataSource>()
            .WithPublishType<IPublishableDataSource>()
            .WithResultType<IDataSourceDetails>();

        /// <summary>
        /// Gets the workbooks <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Workbooks = new MigrationPipelineContentType<IWorkbook>()
            .WithPublishType<IPublishableWorkbook>()
            .WithResultType<IWorkbookDetails>();

        /// <summary>
        /// Gets the views <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType Views = new MigrationPipelineContentType<IView>();

        /// <summary>
        /// Gets the Server to Cloud extract refresh tasks <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType ServerToCloudExtractRefreshTasks = new MigrationPipelineContentType<IServerExtractRefreshTask>()
            .WithPublishType<ICloudExtractRefreshTask>();

        /// <summary>
        /// Gets the custom views <see cref="MigrationPipelineContentType"/>.
        /// </summary>
        public static readonly MigrationPipelineContentType CustomViews = new MigrationPipelineContentType<ICustomView>()
            .WithPublishType<IPublishableCustomView>();

        /// <summary>
        /// Gets the publish type.
        /// </summary>
        public Type PublishType { get; private init; } = ContentType;

        /// <summary>
        /// Gets the result type.
        /// </summary>
        public Type ResultType { get; private init; } = ContentType;

        /// <summary>
        /// Gets the types for this instance.
        /// </summary>
        public IImmutableList<Type> Types => new[] { ContentType, PublishType, ResultType }.Distinct().ToImmutableArray();

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified publish type.
        /// </summary>
        /// <param name="publishType">The publish type.</param>
        public MigrationPipelineContentType WithPublishType(Type publishType)
            => new(ContentType) { PublishType = publishType, ResultType = ResultType };

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified publish type.
        /// </summary>
        public MigrationPipelineContentType WithPublishType<TPublish>()
            => WithPublishType(typeof(TPublish));

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified result type.
        /// </summary>
        /// <param name="resultType">The result type.</param>
        public MigrationPipelineContentType WithResultType(Type resultType)
            => new(ContentType) { PublishType = PublishType, ResultType = resultType };

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance with the specified result type.
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

        private static bool HasInterface(Type t, Type @interface)
            => t.GetInterfaces().Contains(@interface);
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
