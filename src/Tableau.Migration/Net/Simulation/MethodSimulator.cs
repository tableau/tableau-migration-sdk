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

using Tableau.Migration.Net.Simulation.Requests;
using Tableau.Migration.Net.Simulation.Responses;

namespace Tableau.Migration.Net.Simulation
{
    /// <summary>
    /// Interface for an object that simulates an API method and can produce simulated HTTP responses for a certain request path.
    /// </summary>
    /// <param name="RequestMatcher">The request matcher.</param>
    /// <param name="ResponseBuilder">The response builder.</param>
    public record MethodSimulator(IRequestMatcher RequestMatcher, IResponseBuilder ResponseBuilder)
    {
        /// <summary>
        /// Gets the override response builder.
        /// </summary>
        public IResponseBuilder? ResponseOverride { get; set; }

        /// <summary>
        /// Clears the current override response builder.
        /// </summary>
        /// <returns>The current method simulator, for fluent API usage.</returns>
        public MethodSimulator ClearResponseOverride()
        {
            ResponseOverride = null;
            return this;
        }

        /// <summary>
        /// Returns a string which represents the object instance.
        /// </summary>
        public override string ToString() => $"{nameof(RequestMatcher)}: {RequestMatcher}, {nameof(ResponseBuilder)}: {ResponseBuilder}, {nameof(ResponseOverride)}: {ResponseOverride?.ToString() ?? "<null>"}";
    }
}