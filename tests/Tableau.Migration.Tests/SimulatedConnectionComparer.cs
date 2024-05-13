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
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Tests
{
    internal class SimulatedConnectionComparer : ComparerBase<SimulatedConnection>
    {
        public static SimulatedConnectionComparer Instance = new();

        protected override int CompareItems(SimulatedConnection x, SimulatedConnection y)
        {
            Guard.AgainstNull(x.Credentials, nameof(x.Credentials));
            Guard.AgainstNull(y.Credentials, nameof(y.Credentials));

            int result = StringComparer.Ordinal.Compare(x.ServerAddress, y.ServerAddress);

            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.ServerPort, y.ServerPort);
            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.Credentials.Name, y.Credentials.Name);
            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.Credentials.Embed, y.Credentials.Embed);
            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.Credentials.OAuth, y.Credentials.OAuth);
            if (result != 0) return result;

            //Do not test password, we don't have it, and because of encryption we can't verify anyway

            return 0;


        }
    }
}
