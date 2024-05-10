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
using AutoFixture;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineContentTypeExtensionsTests
    {
        public abstract class MigrationPipelineContentTypeExtensionsTest : AutoFixtureTestBase
        {
            private static readonly IFixture _fixture = CreateFixture();

            protected static MigrationPipelineContentType CreateContentType(
                Type contentType,
                Type? publishType = null,
                Type? resultType = null)
                => new MigrationPipelineContentType(contentType)
                    .WithPublishType(publishType ?? _fixture.Create<Type>())
                    .WithResultType(resultType ?? _fixture.Create<Type>());
        }

        public class WithContentTypeInterface : MigrationPipelineContentTypeExtensionsTest
        {
            protected static readonly IEnumerable<MigrationPipelineContentType> ContentTypes = new[]
            {
                CreateContentType(typeof(TestContentType)),
                CreateContentType(typeof(object))
            };

            public class NonGeneric
            {
                [Fact]
                public void Finds_types()
                {
                    var results = ContentTypes.WithContentTypeInterface(typeof(IContentReference));

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }

            public class Generic
            {
                [Fact]
                public void Finds_types()
                {
                    var results = ContentTypes.WithContentTypeInterface<IContentReference>();

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }
        }

        public class WithPublishTypeInterface : MigrationPipelineContentTypeExtensionsTest
        {
            protected static readonly IEnumerable<MigrationPipelineContentType> ContentTypes = new[]
            {
                CreateContentType(typeof(TestContentType)),
                CreateContentType(typeof(object), publishType: typeof(TestContentType))
            };

            public class NonGeneric
            {
                [Fact]
                public void Finds_types()
                {
                    var results = ContentTypes.WithPublishTypeInterface(typeof(IContentReference));

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }

            public class Generic
            {
                [Fact]
                public void Finds_types()
                {
                    var results = ContentTypes.WithPublishTypeInterface<IContentReference>();

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType) }, result);
                }
            }
        }

        public class WithPostPublishTypeInterface : MigrationPipelineContentTypeExtensionsTest
        {
            protected static readonly IEnumerable<MigrationPipelineContentType> ContentTypes = new[]
            {
                CreateContentType(typeof(TestContentType)),
                CreateContentType(typeof(object), publishType: typeof(TestContentType), resultType: typeof(float))
            };

            public class NonGeneric
            {
                [Fact]
                public void Finds_types()
                {
                    var results = ContentTypes.WithPostPublishTypeInterface(typeof(IContentReference));

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType), typeof(float) }, result);
                }
            }

            public class Generic
            {
                [Fact]
                public void Finds_types()
                {
                    var results = ContentTypes.WithPostPublishTypeInterface<IContentReference>();

                    var result = Assert.Single(results);

                    Assert.Equal(new[] { typeof(TestContentType), typeof(float) }, result);
                }
            }
        }
    }
}
