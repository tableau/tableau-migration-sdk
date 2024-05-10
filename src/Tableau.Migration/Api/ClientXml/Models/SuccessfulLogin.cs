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
using System.Xml.Serialization;

namespace Tableau.Migration.Api.ClientXml.Models
{
    [XmlType("successful_login")]
    internal class SuccessfulLogin
    {
        [XmlElement("authenticity_token")]
        public string? AuthenticityToken { get; set; }

        [XmlElement("workgroup_session_id")]
        public string? WorkgroupSessionId { get; set; }

        [XmlElement("user")]
        public UserType? User { get; set; }

        [XmlElement("settings")]
        public SettingsType? Settings { get; set; }

        [XmlElement("error")]
        public ErrorType? Error { get; set; }

        public class ErrorType
        {
            [XmlArray("sites")]
            [XmlArrayItem("site")]
            public SiteType[] Sites { get; set; } = Array.Empty<SiteType>();

            public class SiteType
            {
                [XmlAttribute("id")]
                public string? UrlNamespace { get; set; }

                [XmlText]
                public string? DisplayName { get; set; }
            }
        }

        public class UserType
        {
            [XmlElement("id")]
            public int Id { get; set; }

            [XmlElement("name")]
            public string? Name { get; set; }

            [XmlElement("friendly_name")]
            public string? FriendlyName { get; set; }

            [XmlElement("administrator")]
            public bool IsAdministrator { get; set; }

            [XmlElement("site_namespace")]
            public string? SiteUrlNamespace { get; set; }

            [XmlElement("site_prefix")]
            public string? SitePrefix { get; set; }

            [XmlElement("site_displayname")]
            public string? SiteDisplayName { get; set; }

            [XmlElement("admin_type")]
            public string? AdminTypeValue { get; set; }
        }

        public class SettingsType
        {
            [XmlElement("saas_enabled")]
            public bool SaasEnabled { get; set; }
        }
    }
}
