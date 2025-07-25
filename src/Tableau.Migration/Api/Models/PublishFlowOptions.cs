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

using System;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Class for API client prep flow publish options. 
    /// </summary>
    public class PublishFlowOptions : PublishFileOptionsBase, IPublishFlowOptions
    {
        ///<inheritdoc/>
        public string Name { get; }

        ///<inheritdoc/>
        public string Description { get; }

        ///<inheritdoc/>
        public bool Overwrite { get; } = true;

        ///<inheritdoc/>
        public Guid ProjectId { get; }

        /// <summary>
        /// Creates a new <see cref="PublishFlowOptions"/> instance.
        /// </summary>
        /// <param name="flow">The publishable prep flow information.</param>
        /// <param name="fileType">The type of prep flow file.</param>
        public PublishFlowOptions(IPublishableFlow flow, string fileType = FlowFileTypes.Tflx)
              : base(flow.File, fileType)
        {
            Name = flow.Name;
            Description = flow.Description;
            ProjectId = ((IContainerContent)flow).Container.Id;
        }
    }
}
