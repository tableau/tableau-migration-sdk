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

using System.Collections.Generic;
using AutoFixture;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class UpdateConnectionRequestTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var request = AutoFixture.Create<UpdateConnectionRequest>();

                var connection = request.Connection;
                Assert.NotNull(connection);

                var serialized = Serializer.SerializeToXml(request);

                var expected = $@"
<tsRequest>
  <connection
    serverAddress=""{connection.ServerAddress}"" 
    serverPort=""{connection.ServerPort}""
    userName=""{connection.ConnectionUsername}"" 
    password=""{connection.Password}""
    embedPassword=""{connection.EmbedPassword}""
	queryTaggingEnabled=""{connection.QueryTaggingEnabled}"" />
</tsRequest>";
                AssertXmlEqual(expected, serialized);

            }

            [Fact]
            public void Serializes_with_null_QueryTaggingEnabled()
            {
                var request = AutoFixture.Create<UpdateConnectionRequest>();

                var connection = request.Connection;
                Assert.NotNull(connection);

                connection.QueryTaggingEnabledFlag = null;
                var serialized = Serializer.SerializeToXml(request);

                var expected = $@"
<tsRequest>
  <connection
    serverAddress=""{connection.ServerAddress}"" 
    serverPort=""{connection.ServerPort}""
    userName=""{connection.ConnectionUsername}"" 
    password=""{connection.Password}""
    embedPassword=""{connection.EmbedPassword}"" />
</tsRequest>";
                AssertXmlEqual(expected, serialized);

            }

            [Fact]
            public void Serializes_with_null_EmbedPassWord()
            {
                var request = AutoFixture.Create<UpdateConnectionRequest>();

                var connection = request.Connection;
                Assert.NotNull(connection);

                connection.EmbedPasswordFlag = null;
                var serialized = Serializer.SerializeToXml(request);

                var expected = $@"
<tsRequest>
  <connection
    serverAddress=""{connection.ServerAddress}"" 
    serverPort=""{connection.ServerPort}""
    userName=""{connection.ConnectionUsername}"" 
    password=""{connection.Password}""   
	queryTaggingEnabled=""{connection.QueryTaggingEnabled}"" />
</tsRequest>";
                AssertXmlEqual(expected, serialized);

            }
        }

        public class Ctor : AutoFixtureTestBase
        {
            #region - Test Data -

            private const string TEST_SERVER_ADDRESS = "127.0.0.1";
            private const string TEST_SERVER_PORT = "8080";
            private const string TEST_USERNAME = "user1";
            private const string TEST_PASSWORD = "this-is-a-str!ng";

            public class RequestTestData
            {
                public RequestTestData(
                    IUpdateConnectionOptions options,
                    UpdateConnectionRequest.ConnectionType expectedConnectionType)
                {
                    Options = options;
                    ExpectedConnectionType = expectedConnectionType;
                }

                public IUpdateConnectionOptions Options { get; }

                public UpdateConnectionRequest.ConnectionType ExpectedConnectionType { get; }
            }

            public static IEnumerable<object[]> GetRequestTestData()
            {
                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(
                           serverAddress: TEST_SERVER_ADDRESS,
                           serverPort: TEST_SERVER_PORT,
                           connectionUsername: TEST_USERNAME,
                           password: TEST_PASSWORD,
                           embedPassword: false,
                           queryTaggingEnabled: false),
                       expectedConnectionType: new()
                       {
                           ServerAddress= TEST_SERVER_ADDRESS,
                           ServerPort=TEST_SERVER_PORT,
                           ConnectionUsername=TEST_USERNAME,
                           Password=TEST_PASSWORD,
                           EmbedPasswordFlag = false,
                           QueryTaggingEnabledFlag=false
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(
                           serverAddress: TEST_SERVER_ADDRESS,
                           serverPort: TEST_SERVER_PORT,
                           connectionUsername: TEST_USERNAME,
                           password: TEST_PASSWORD,
                           embedPassword: true,
                           queryTaggingEnabled: false),
                       expectedConnectionType: new()
                       {
                           ServerAddress= TEST_SERVER_ADDRESS,
                           ServerPort=TEST_SERVER_PORT,
                           ConnectionUsername=TEST_USERNAME,
                           Password=TEST_PASSWORD,
                           EmbedPasswordFlag = true,
                           QueryTaggingEnabledFlag=false
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(
                           serverAddress: TEST_SERVER_ADDRESS,
                           serverPort: TEST_SERVER_PORT,
                           connectionUsername: TEST_USERNAME,
                           password: TEST_PASSWORD,
                           embedPassword: false,
                           queryTaggingEnabled: true),
                       expectedConnectionType: new()
                       {
                           ServerAddress= TEST_SERVER_ADDRESS,
                           ServerPort=TEST_SERVER_PORT,
                           ConnectionUsername=TEST_USERNAME,
                           Password=TEST_PASSWORD,
                           EmbedPasswordFlag = false,
                           QueryTaggingEnabledFlag=true
                       })
                };

                yield return new object[]
                {
                    new RequestTestData(
                       options: new UpdateConnectionOptions(
                           serverAddress: TEST_SERVER_ADDRESS,
                           serverPort: TEST_SERVER_PORT,
                           connectionUsername: TEST_USERNAME,
                           password: TEST_PASSWORD,
                           embedPassword: false),
                       expectedConnectionType: new()
                       {
                           ServerAddress= TEST_SERVER_ADDRESS,
                           ServerPort=TEST_SERVER_PORT,
                           ConnectionUsername=TEST_USERNAME,
                           Password=TEST_PASSWORD,
                           EmbedPasswordFlag = false
                       })
                };

                yield return new object[]
                {
                    new RequestTestData(
                       options: new UpdateConnectionOptions(
                           serverAddress: TEST_SERVER_ADDRESS,
                           serverPort: TEST_SERVER_PORT,
                           connectionUsername: TEST_USERNAME,
                           password: TEST_PASSWORD,
                           embedPassword: false),
                       expectedConnectionType: new()
                       {
                           ServerAddress= TEST_SERVER_ADDRESS,
                           ServerPort=TEST_SERVER_PORT,
                           ConnectionUsername=TEST_USERNAME,
                           Password=TEST_PASSWORD,
                           EmbedPasswordFlag = false
                       })
                };
                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(serverAddress: TEST_SERVER_ADDRESS),
                       expectedConnectionType: new()
                       {
                           ServerAddress= TEST_SERVER_ADDRESS,
                           EmbedPasswordFlag = null,
                           QueryTaggingEnabledFlag = null
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(serverPort: TEST_SERVER_PORT),
                       expectedConnectionType: new()
                       {
                           ServerPort=TEST_SERVER_PORT,
                           EmbedPasswordFlag = null,
                           QueryTaggingEnabledFlag = null
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(connectionUsername: TEST_USERNAME),
                       expectedConnectionType: new()
                       {
                           ServerAddress= null,

                           ConnectionUsername=TEST_USERNAME,

                           EmbedPasswordFlag = null,
                           QueryTaggingEnabledFlag = null
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(password: TEST_PASSWORD),
                       expectedConnectionType: new()
                       {
                           Password=TEST_PASSWORD,
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(embedPassword: false),
                       expectedConnectionType: new()
                       {
                           EmbedPasswordFlag = false,
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(embedPassword: true),
                       expectedConnectionType: new()
                       {
                           EmbedPasswordFlag = true,
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(queryTaggingEnabled: false),
                       expectedConnectionType: new()
                       {
                           QueryTaggingEnabledFlag = false,
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(queryTaggingEnabled: true),
                       expectedConnectionType: new()
                       {
                           QueryTaggingEnabledFlag = true,
                       })
                };

                yield return new object[]
                {
                   new RequestTestData(
                       options: new UpdateConnectionOptions(),
                       expectedConnectionType: new())
                };
            }

            #endregion

            [Theory]
            [MemberData(nameof(GetRequestTestData))]
            public void BuildsRequest(RequestTestData testData)
            {
                var result = new UpdateConnectionRequest.ConnectionType(testData.Options);

                Assert.Equal(testData.ExpectedConnectionType.ServerAddress, result.ServerAddress);
                Assert.Equal(testData.ExpectedConnectionType.ServerPort, result.ServerPort);
                Assert.Equal(testData.ExpectedConnectionType.ConnectionUsername, result.ConnectionUsername);
                Assert.Equal(testData.ExpectedConnectionType.Password, result.Password);
                Assert.Equal(testData.ExpectedConnectionType.EmbedPassword, result.EmbedPassword);
                Assert.Equal(testData.ExpectedConnectionType.QueryTaggingEnabled, result.QueryTaggingEnabled);
            }

        }
    }
}
