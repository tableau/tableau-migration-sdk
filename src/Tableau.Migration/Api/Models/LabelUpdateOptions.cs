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

using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Implementation for a content item's label update options.
    /// </summary>
    public class LabelUpdateOptions : ILabelUpdateOptions
    {
        /// <summary>
        /// Constructor to build from <see cref="ILabel"/>
        /// </summary>
        /// <param name="label"></param>
        public LabelUpdateOptions(ILabel label)
        {
            Value = label.Value;
            Message = label.Message;
            Active = label.Active;
            Elevated = label.Elevated;
        }

        /// <inheritdoc/>
        public string? Value { get; set; }

        /// <inheritdoc/>
        public string? Message { get; set; }

        /// <inheritdoc/>
        public bool Active { get; set; }

        /// <inheritdoc/>
        public bool Elevated { get; set; }
    }
}
