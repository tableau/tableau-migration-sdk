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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.ClientXml.Models
{
    /// <summary>
    /// Quasi union type of <see cref="SuccessfulLogin"/> and <see cref="FailedLogin"/>.
    /// </summary>
    internal class SuccessfulOrFailedLogin : IXmlSerializable
    {
        internal const string SUCCESSFUL_LOGIN_LOCAL_NAME = "successful_login";
        internal const string FAILED_LOGIN_LOCAL_NAME = "error";

        public SuccessfulLogin? SuccessfulLogin { get; set; }
        public FailedLogin? FailedLogin { get; set; }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName == SUCCESSFUL_LOGIN_LOCAL_NAME)
            {
                SuccessfulLogin = (SuccessfulLogin?)new XmlSerializer(typeof(SuccessfulLogin)).Deserialize(reader);
                FailedLogin = null;
            }
            else if (reader.LocalName == FAILED_LOGIN_LOCAL_NAME)
            {
                SuccessfulLogin = null;
                FailedLogin = (FailedLogin?)new XmlSerializer(typeof(FailedLogin)).Deserialize(reader);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(SuccessfulOrFailedLogin)} cannot deserialize root element named \"{reader.LocalName}\"");
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException($"{nameof(SuccessfulOrFailedLogin)} only supports serialization, not deserialization");
        }
    }
}
