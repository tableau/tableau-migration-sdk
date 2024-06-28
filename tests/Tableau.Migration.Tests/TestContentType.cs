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

namespace Tableau.Migration.Tests
{
    public class TestContentType : IContentReference
    {
        public Guid Id { get; set; }

        public string ContentUrl { get; set; } = string.Empty;

        public ContentLocation Location { get; set; }

        public string Name => Location.Name;

        public TestContentType()
        { }

        public TestContentType(IContentReference reference)
        {
            Id = reference.Id;
            ContentUrl = reference.ContentUrl;
            Location = reference.Location;
        }

        public bool Equals(IContentReference? other)
        {
            return other is not null &&
                   Id.Equals(other.Id) &&
                   ContentUrl == other.ContentUrl &&
                   Location.Equals(other.Location) &&
                   Name == other.Name;
        }
    }

    public class OtherTestContentType : IContentReference
    {
        public Guid Id { get; set; }

        public string ContentUrl { get; set; } = string.Empty;

        public ContentLocation Location { get; set; }

        public string Name => Location.Name;

        public bool Equals(IContentReference? other)
        {
            throw new NotImplementedException();
        }
    }
}
