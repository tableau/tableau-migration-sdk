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

using System.Collections.Immutable;
using System.Linq;

namespace Tableau.Migration.PythonGenerator
{
    internal sealed record PythonDocstring(string Summary, ImmutableArray<PythonArgDocstring> Args, string? Returns = null)
    {
        /// <summary>
        /// Gets whether or not the docstring has "extra info,"
        /// i.e. more than just a summary.
        /// </summary>
        public bool HasExtraInfo => HasReturns || HasArgs;

        /// <summary>
        /// Gets whether or not the docstring has a string for a return value.
        /// </summary>
        public bool HasReturns => !string.IsNullOrWhiteSpace(Returns);

        /// <summary>
        /// Gets whether or not the docstring has a strings for a arguments.
        /// </summary>
        public bool HasArgs => Args.Any();

        public PythonDocstring(string summary)
            : this(summary, ImmutableArray<PythonArgDocstring>.Empty)
        { }

        public PythonDocstring(string summary, string returns, params (string ArgName, string ArgValue)[] args)
            : this(summary, args.Select(a => new PythonArgDocstring(a.ArgName, a.ArgValue)).ToImmutableArray(), returns)
        { }
    }
}
