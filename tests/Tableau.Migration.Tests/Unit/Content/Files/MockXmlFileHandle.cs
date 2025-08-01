﻿//
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

using System.IO;
using System.Threading;
using System.Xml.Linq;
using Moq;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class MockXmlFileHandle : Mock<IContentFileHandle>
    {
        public XDocument Xml { get; set; }

        public Mock<ITableauFileXmlStream> MockXmlStream { get; set; }

        public MockXmlFileHandle(string? xml = null)
            : base()
        {
            Xml = xml is null ? new() : XDocument.Parse(xml);

            MockXmlStream = new();
            MockXmlStream.Setup(x => x.GetXmlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Xml);

            Setup(x => x.GetXmlStreamAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => MockXmlStream.Object);

            Setup(x => x.OpenReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new ContentFileStream(new MemoryStream(Constants.DefaultEncoding.GetBytes(Xml.ToString()))));
        }
    }
}
