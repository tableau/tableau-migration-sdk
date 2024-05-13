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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class CallbackHookWrapperTests
    {
        [Fact]
        public async Task CallsCallbackAsync()
        {
            int calls = 0;
            var callbackResult = MigrationActionResult.Succeeded();

            Task<IMigrationActionResult?> callback(IMigrationActionResult ctx, CancellationToken cancel)
            {
                calls++;
                return Task.FromResult<IMigrationActionResult?>(callbackResult);
            }

            var hook = new CallbackHookWrapper<IMigrationActionCompletedHook, IMigrationActionResult>(callback);

            var result = await hook.ExecuteAsync(MigrationActionResult.Succeeded(), default);

            Assert.Same(callbackResult, result);
            Assert.Equal(1, calls);
        }
    }
}
