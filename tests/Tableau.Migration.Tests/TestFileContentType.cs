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
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Tests.Unit.Content.Files;

namespace Tableau.Migration.Tests
{
    public class TestFileContentType : TestPublishType, IFileContent
    {
        public bool IsDisposed { get; private set; }

        public TestFileContentType()
            : this(new MockXmlFileHandle().Object)
        { }

        public TestFileContentType(IContentFileHandle file)
        {
            File = file;
        }

        public IContentFileHandle File { get; set; }

        public async ValueTask DisposeAsync()
        {
            await File.DisposeAsync();
            IsDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
