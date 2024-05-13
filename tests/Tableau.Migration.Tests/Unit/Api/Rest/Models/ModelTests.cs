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
using System.Reflection;
using System.Xml.Serialization;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class ModelTests
    {
        public abstract class ModelTest : AutoFixtureTestBase
        {
            protected static readonly IImmutableList<Type> ModelTypes = typeof(TableauServerResponse).Assembly.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                !t.IsInterface &&
                (t.IsAssignableTo(typeof(TableauServerResponse)) || t.IsAssignableTo(typeof(TableauServerRequest)) || t.IsAssignableTo(typeof(PagedTableauServerResponse<>))))
                .ToImmutableList();

            protected static readonly IImmutableList<Type> RequestModelTypes = ModelTypes
                .Where(t => t.IsAssignableTo(typeof(TableauServerRequest)))
                .ToImmutableList();

            protected static readonly IImmutableList<Type> ResponseModelTypes = ModelTypes
                .Where(t => t.IsAssignableTo(typeof(TableauServerResponse)))
                .ToImmutableList();

            protected static readonly IImmutableList<Type> PagedResponseModelTypes = ModelTypes
                .Where(t => t.IsAssignableTo(typeof(PagedTableauServerResponse<>)))
                .ToImmutableList();
        }

        public class XmlTypeNames : ModelTest
        {
            [Fact]
            public void Response_types_have_xml_type_name()
            {
                foreach (var type in ResponseModelTypes.Concat(PagedResponseModelTypes))
                    AssertXmlTypeName(type, TableauServerResponse.XmlTypeName);
            }

            [Fact]
            public void Response_type_items_have_xml_element_name()
            {
                foreach (var type in ResponseModelTypes)
                {
                    if (HasResponseItemType(type))
                    {
                        var itemProperty = type.GetProperty(nameof(ITableauServerResponse<object>.Item));
                        Assert.NotNull(itemProperty);
                        AssertXmlElementNameExists(itemProperty, type);
                    }
                }

                static bool HasResponseItemType(Type t)
                {
                    return t.GetInterfaces().Any(x =>
                      x.IsGenericType &&
                      x.GetGenericTypeDefinition() == typeof(ITableauServerResponse<>));
                }
            }

            [Fact]
            public void Paged_response_type_items_have_xml_element_name()
            {
                foreach (var type in PagedResponseModelTypes)
                {
                    if (HasResponseItemsType(type))
                    {
                        var itemProperty = type.GetProperty(nameof(ITableauServerListResponse<object>.Items));
                        Assert.NotNull(itemProperty);
                        AssertXmlElementNameExists(itemProperty, type);
                    }
                }

                static bool HasResponseItemsType(Type t)
                {
                    return t.GetInterfaces().Any(x =>
                      x.IsGenericType &&
                      x.GetGenericTypeDefinition() == typeof(ITableauServerListResponse<>));
                }
            }

            [Fact]
            public void Request_types_have_xml_type_name()
            {
                foreach (var type in RequestModelTypes)
                    AssertXmlTypeName(type, TableauServerRequest.XmlTypeName);
            }

            private static void AssertXmlTypeName(Type type, string expectedTypeName)
            {
                var xmlTypeAttribute = type.GetCustomAttribute<XmlTypeAttribute>();
                Assert.True(xmlTypeAttribute is not null, $"{type.Name} must have an XML type attribute.");
                Assert.Equal(expectedTypeName, xmlTypeAttribute.TypeName);
            }

            private static void AssertXmlElementNameExists(PropertyInfo property, Type type)
            {
                var xmlElementAttribute = property.GetCustomAttribute<XmlElementAttribute>();
                Assert.True(xmlElementAttribute != null, $"{type.Name} does not have an XmlElementAttribute");
            }
        }

        public class Serialization : ModelTest
        {
            [Fact]
            public void Serializes_type_name()
            {
                foreach (var type in ModelTypes)
                {
                    var model = Create(type);

                    var xml = model.ToXml();

                    var expectedTypeName = model is TableauServerResponse ?
                                           TableauServerResponse.XmlTypeName
                                           : TableauServerRequest.XmlTypeName;

                    var trimmedXml = xml.Trim('<').Trim('>').Trim('/').Trim();

                    Assert.StartsWith(expectedTypeName, trimmedXml);
                    Assert.EndsWith(expectedTypeName, trimmedXml);
                }
            }
        }
    }
}
