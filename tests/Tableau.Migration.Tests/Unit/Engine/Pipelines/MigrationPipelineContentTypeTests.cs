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
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineContentTypeTests
    {
        public abstract class MigrationPipelineContentTypeTest : AutoFixtureTestBase
        {
            protected Type CreateType() => Create<Type>();

            protected IImmutableList<Type> CreateTypes(int createCount, params Type[] explicitTypes)
                => explicitTypes.Concat(CreateMany<Type>(createCount)).ToImmutableArray();

            protected MigrationPipelineContentType CreateContentType(Type? contentType = null) => new(contentType ?? CreateType());

            protected static void AssertTypes(MigrationPipelineContentType result, Type contentType, Type prepareType, Type publishType, Type resultType)
            {
                Assert.Same(contentType, result.ContentType);
                Assert.Same(prepareType, result.PrepareType);
                Assert.Same(publishType, result.PublishType);
                Assert.Same(resultType, result.ResultType);
            }
        }

        public class Ctor : MigrationPipelineContentTypeTest
        {
            [Fact]
            public void Content_only()
            {
                var type = CreateType();

                var t = new MigrationPipelineContentType(type);

                AssertTypes(t, type, type, type, type);
            }
        }

        public class WithPrepareType : MigrationPipelineContentTypeTest
        {
            [Fact]
            public void Different_types()
            {
                var contentType = CreateType();
                var prepareType = CreateType();

                var t = CreateContentType(contentType).WithPrepareType(prepareType);

                AssertTypes(t, contentType, prepareType, contentType, contentType);
            }

            [Fact]
            public void Same_types()
            {
                var type = CreateType();

                var t = CreateContentType(type).WithPrepareType(type);

                AssertTypes(t, type, type, type, type);
            }
        }

        public class WithPublishType : MigrationPipelineContentTypeTest
        {
            [Fact]
            public void Different_types()
            {
                var contentType = CreateType();
                var publishType = CreateType();

                var t = CreateContentType(contentType).WithPublishType(publishType);

                AssertTypes(t, contentType, contentType, publishType, contentType);
            }

            [Fact]
            public void Same_types()
            {
                var type = CreateType();

                var t = CreateContentType(type).WithPublishType(type);

                AssertTypes(t, type, type, type, type);
            }
        }

        public class WithResultType : MigrationPipelineContentTypeTest
        {
            [Fact]
            public void Different_types()
            {
                var contentType = CreateType();
                var resultType = CreateType();

                var t = CreateContentType(contentType).WithResultType(resultType);

                AssertTypes(t, contentType, contentType, contentType, resultType);
            }

            [Fact]
            public void Same_types()
            {
                var type = CreateType();

                var t = CreateContentType(type).WithResultType(type);

                AssertTypes(t, type, type, type, type);
            }
        }

        public class GetContentTypeForInterface : MigrationPipelineContentTypeTest
        {
            [Fact]
            public void Returns_null_when_not_found()
            {
                var t = new MigrationPipelineContentType(typeof(object));

                Assert.Null(t.GetContentTypeForInterface(typeof(IContentReference)));
            }

            [Fact]
            public void Returns_content_type_when_found()
            {
                var type = typeof(TestContentType);

                var t = new MigrationPipelineContentType(type);

                Assert.Equal([type], t.GetContentTypeForInterface(typeof(IContentReference)));
            }
        }

        public class GetPublishTypeForInterface : MigrationPipelineContentTypeTest
        {
            [Fact]
            public void Returns_null_when_not_found()
            {
                var t = new MigrationPipelineContentType(typeof(TestContentType)).WithPublishType(typeof(object));

                Assert.Null(t.GetPublishTypeForInterface(typeof(IContentReference)));
            }

            [Fact]
            public void Returns_publish_type_when_found()
            {
                var type = typeof(TestContentType);

                var t = new MigrationPipelineContentType(typeof(object)).WithPublishType(type);

                Assert.Equal([type], t.GetPublishTypeForInterface(typeof(IContentReference)));
            }
        }

        public class GetPostPublishTypesForInterface : MigrationPipelineContentTypeTest
        {
            [Fact]
            public void Returns_null_when_not_found()
            {
                var t = new MigrationPipelineContentType(typeof(TestContentType)).WithPublishType(typeof(object));

                Assert.Null(t.GetPostPublishTypesForInterface(typeof(IContentReference)));
            }

            [Fact]
            public void Returns_publish_type_when_found()
            {
                var type = typeof(TestContentType);

                var t = new MigrationPipelineContentType(typeof(object)).WithPublishType(type).WithResultType(typeof(int));

                Assert.Equal([type, typeof(int)], t.GetPostPublishTypesForInterface(typeof(IContentReference)));
            }
        }

        public class GetConfigKey
        {
            private static void AssertConfigKey(MigrationPipelineContentType pipelineContentType, string actualConfigKey)
            {
                var contentType = pipelineContentType.ContentType;

                if (!contentType.GenericTypeArguments.Any())
                {
                    Assert.Equal(contentType.Name, $"I{actualConfigKey}");
                    return;
                }

                var cleanedTypeName = contentType.Name.TrimEnd('1').TrimEnd('`');
                Assert.StartsWith(cleanedTypeName, $"I{actualConfigKey}");

                foreach (var arg in contentType.GenericTypeArguments)
                {
                    Assert.Contains($"_{arg.Name.TrimStart('I')}", actualConfigKey);
                }
            }

            [Fact]
            public void ReturnsConfigKey()
            {
                var pipelineContentTypes = MigrationPipelineContentType.GetAllMigrationPipelineContentTypes();

                foreach (var pipelineContentType in pipelineContentTypes)
                {
                    Assert.NotNull(pipelineContentType);
                    var actualConfigKey = pipelineContentType.GetConfigKey();

                    AssertConfigKey(pipelineContentType, actualConfigKey);
                }
            }

            [Fact]
            public void StaticType()
            {
                var pipelineContentTypes = MigrationPipelineContentType.GetAllMigrationPipelineContentTypes();

                foreach (var pipelineContentType in pipelineContentTypes)
                {
                    Assert.NotNull(pipelineContentType);
                    var actualConfigKey = MigrationPipelineContentType.GetConfigKeyForType(pipelineContentType.ContentType);

                    AssertConfigKey(pipelineContentType, actualConfigKey);
                }
            }
        }

        public class GetMigrationPipelineContentTypes
        {
            public static TheoryData<PipelineProfile, ImmutableArray<MigrationPipelineContentType>> GetPipelineProfiles()
            {
                return new TheoryData<PipelineProfile, ImmutableArray<MigrationPipelineContentType>>
                {
                    { PipelineProfile.ServerToCloud, ServerToCloudMigrationPipeline.ContentTypes },
                    { PipelineProfile.ServerToServer, ServerToServerMigrationPipeline.ContentTypes },
                    { PipelineProfile.CloudToCloud, CloudToCloudMigrationPipeline.ContentTypes }
                };
            }

            [Theory]
            [MemberData(nameof(GetPipelineProfiles))]
            public void ReturnsContentTypes(PipelineProfile profile, ImmutableArray<MigrationPipelineContentType> contentTypes)
            {
                var pipelineContentTypes = MigrationPipelineContentType.GetMigrationPipelineContentTypes(profile);
                Assert.Equal(contentTypes.Length, pipelineContentTypes.Length);
                for (var i = 0; i < contentTypes.Length; i++)
                {
                    Assert.Same(contentTypes[i], pipelineContentTypes[i]);
                }
            }
        }
    }
}
