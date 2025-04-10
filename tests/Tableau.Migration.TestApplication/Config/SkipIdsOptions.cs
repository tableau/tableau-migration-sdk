//
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

using System;

namespace Tableau.Migration.TestApplication.Config
{
    public class SkipIdsOptions
    {
        #region Json Properties

        public string[] users { get; set; } = Array.Empty<string>();

        public string[] groups { get; set; } = Array.Empty<string>();

        public string[] projects { get; set; } = Array.Empty<string>();

        public string[] workbooks { get; set; } = Array.Empty<string>();

        public string[] datasources { get; set; } = Array.Empty<string>();

        #endregion

        #region Helper Properties

        public Guid[] UserGuids
        {
            get
            {
                return Array.ConvertAll(users, Guid.Parse);
            }
        }

        public Guid[] GroupGuids
        {
            get
            {
                return Array.ConvertAll(groups, Guid.Parse);
            }
        }

        public Guid[] ProjectGuids
        {
            get
            {
                return Array.ConvertAll(projects, Guid.Parse);
            }
        }

        public Guid[] WorkbookGuids
        {
            get
            {
                return Array.ConvertAll(workbooks, Guid.Parse);
            }
        }

        public Guid[] DatasourceGuids
        {
            get
            {
                return Array.ConvertAll(datasources, Guid.Parse);
            }
        }

        #endregion
    }
}
