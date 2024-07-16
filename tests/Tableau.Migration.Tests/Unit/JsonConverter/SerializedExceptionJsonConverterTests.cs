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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.JsonConverters;
using Tableau.Migration.JsonConverters.SerializableObjects;
using Xunit;

namespace Tableau.Migration.Tests.Unit.JsonConverter
{
    public class SerializedExceptionJsonConverterTests : AutoFixtureTestBase
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public SerializedExceptionJsonConverterTests()
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            foreach (var converter in MigrationManifestSerializer.CreateConverters())
            {
                _serializerOptions.Converters.Add(converter);
            }
        }

        [Theory]
        [ClassData(typeof(SerializableExceptionTypeData))]
        public void WriteAndReadBack_ExceptionObject_SerializesAndDeserializesToJson(SerializableException ex)
        {
            // Arrange
            var converter = new SerializedExceptionJsonConverter();

            Assert.NotNull(ex.Error);
            var exceptionNamespace = ex.Error.GetType().Namespace;
            Assert.NotNull(exceptionNamespace);

            if (!exceptionNamespace.StartsWith("System")) // Built in Exception is not equatable
            {
                // We require all custom exception to be equatable so we can test serializability. 
                Assert.True(ex.Error.ImplementsEquatable());
            }

            // Serialize
            using (var memoryStream = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(memoryStream);
                converter.Write(writer, ex, _serializerOptions);
                writer.Flush();
                var json = Encoding.UTF8.GetString(memoryStream.ToArray());

                var expectedJsonMessage = ex.Error?.Message.Replace("'", "\\u0027") ?? "";

                // Assert Writer
                Assert.NotNull(json);
                Assert.NotEmpty(json);
                Assert.Contains(expectedJsonMessage, json);

                // Deserialize
                var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
                var result = converter.Read(ref reader, typeof(Exception), _serializerOptions);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Error);
                Assert.Equal(ex.Error!.Message, result.Error.Message);

                if (!exceptionNamespace.StartsWith("System")) // Built in Exception is not equatable
                {
                    Assert.Equal(ex.Error, result.Error);
                }
            }
        }


        /// <summary>
        /// Provides a collection of serializable exception objects.
        /// </summary>
        public class SerializableExceptionTypeData : AutoFixtureTestBase, IEnumerable<object[]>
        {
            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var e in FixtureFactory.CreateErrors(AutoFixture))
                {
                    var ex = new SerializableException(e);
                    yield return new object[] { ex };
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
