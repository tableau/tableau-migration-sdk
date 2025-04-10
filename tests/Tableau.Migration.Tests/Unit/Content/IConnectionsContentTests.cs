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

using System.Collections.Immutable;
using Moq;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class IConnectionsContentTests
    {
        public abstract class IConnectionsContentTest : AutoFixtureTestBase
        {
            protected static IConnectionsContent CreateConnectionsContent(params IConnection[] connections)
            {
                var mockContent = new Mock<IConnectionsContent>() { CallBase = true };
                mockContent.SetupGet(c => c.Connections).Returns(ImmutableArray.Create(connections));
                return mockContent.Object;
            }
        }

        public class HasEmbeddedPassword : IConnectionsContentTest
        {
            protected IConnection CreateConnection(bool embedPassword)
                => Create<Mock<IConnection>>(m =>
                {
                    m.SetupGet(c => c.EmbedPassword).Returns(embedPassword);
                })
                .Object;

            [Fact]
            public void Returns_true_when_any_EmbedPassword_is_true()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(true));

                Assert.True(content.HasEmbeddedPassword);
            }

            [Fact]
            public void Returns_false_when_connections_empty()
            {
                var content = CreateConnectionsContent();

                Assert.False(content.HasEmbeddedPassword);
            }

            [Fact]
            public void Returns_false_when_EmbedPassword_is_false()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(false));

                Assert.False(content.HasEmbeddedPassword);
            }
        }

        public class HasEmbeddedOAuthManagedKeychain : IConnectionsContentTest
        {
            protected IConnection CreateConnection(bool embedPassword, bool useOAuthManagedKeychain)
                => Create<Mock<IConnection>>(m =>
                {
                    m.SetupGet(c => c.EmbedPassword).Returns(embedPassword);
                    m.SetupGet(c => c.UseOAuthManagedKeychain).Returns(useOAuthManagedKeychain);
                })
                .Object;

            [Fact]
            public void Returns_true_when_any_EmbedPassword_is_true_and_UseOAuthManagedKeychain_is_true()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(true, true),
                    CreateConnection(false, false));

                Assert.True(content.HasEmbeddedOAuthManagedKeychain);
            }

            [Fact]
            public void Returns_false_when_connections_empty()
            {
                var content = CreateConnectionsContent();

                Assert.False(content.HasEmbeddedOAuthManagedKeychain);
            }

            [Fact]
            public void Returns_false_when_EmbedPassword_is_false_and_UseOAuthManagedKeychain_is_true()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(false, true));

                Assert.False(content.HasEmbeddedOAuthManagedKeychain);
            }

            [Fact]
            public void Returns_false_when_EmbedPassword_is_true_and_UseOAuthManagedKeychain_is_false()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(true, false));

                Assert.False(content.HasEmbeddedOAuthManagedKeychain);
            }
        }

        public class HasEmbeddedOAuthCredentials : IConnectionsContentTest
        {
            protected IConnection CreateConnection(bool embedPassword, string? authenticationType)
                => Create<Mock<IConnection>>(m =>
                {
                    m.SetupGet(c => c.EmbedPassword).Returns(embedPassword);
                    m.SetupGet(c => c.AuthenticationType).Returns(authenticationType);
                })
                .Object;

            [Fact]
            public void Returns_true_when_any_EmbedPassword_is_true_and_UseOAuthManagedKeychain_is_true()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(true, "oauth"),
                    CreateConnection(false, "not-oauth"));

                Assert.True(content.HasEmbeddedOAuthCredentials);
            }

            [Fact]
            public void Returns_false_when_connections_empty()
            {
                var content = CreateConnectionsContent();

                Assert.False(content.HasEmbeddedOAuthCredentials);
            }

            [Fact]
            public void Returns_false_when_EmbedPassword_is_false_and_UseOAuthManagedKeychain_is_OAuth()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(false, "oauth"));

                Assert.False(content.HasEmbeddedOAuthCredentials);
            }

            [Fact]
            public void Returns_false_when_EmbedPassword_is_true_and_AuthenticationType_is_not_OAuth()
            {
                var content = CreateConnectionsContent(
                    CreateConnection(true, "not-oauth"));

                Assert.False(content.HasEmbeddedOAuthCredentials);
            }
        }
    }
}
