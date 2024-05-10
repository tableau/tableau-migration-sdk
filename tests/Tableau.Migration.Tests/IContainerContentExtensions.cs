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

using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Tests
{
    public static class IContainerContentExtensions
    {
        public static Mock<TItem> WithProject<TItem>(this Mock<TItem> mock, IProjectType project)
            where TItem : class, IContainerContent
        {
            mock.As<IContainerContent>().SetupGet(p => p.Container)
                .Returns(new ContentReferenceStub(project.Id, "", new ContentLocation(), project.Name!));

            return mock;
        }
    }
}
