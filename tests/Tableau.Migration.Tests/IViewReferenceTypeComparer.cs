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
using System.Linq;
using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests
{
    internal class IViewReferenceTypeComparer : ComparerBase<IViewReferenceType>
    {
        public static IViewReferenceTypeComparer Instance = new();

        protected override int CompareItems(IViewReferenceType x, IViewReferenceType y)
        {
            Assert.NotNull(x.ContentUrl);
            Assert.NotNull(y.ContentUrl);

            // The content URL of a view is <workbook name>/<view Name>
            // As the workbook name may change during a migration, we can't rely on this
            // View renames are not supported (outside of xml transformer changes), so this will work 
            // for our testing purposes. 
            var xViewName = x.ContentUrl.Split(Constants.PathSeparator).Last();
            var yViewName = y.ContentUrl.Split(Constants.PathSeparator).Last();

            return StringComparer.Ordinal.Compare(xViewName, yViewName);
        }

    }
}
