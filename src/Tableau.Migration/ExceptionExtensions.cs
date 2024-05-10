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
using System.Threading.Tasks;

namespace Tableau.Migration
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Returns whether or not the exception represents a cancellation of some kind.
        /// </summary>
        /// <param name="exception">The exception to test.</param>
        /// <returns>True if the exception indicates a cancelation, otherwise false.</returns>
        public static bool IsCancellationException(this Exception exception)
        {
            //Test basic types.
            if (exception is OperationCanceledException || exception is TaskCanceledException)
            {
                return true;
            }
            else if (exception is AggregateException aggException) //Test nested task canceled exception.
            {
                return aggException.InnerExceptions.All(inner => inner.IsCancellationException());
            }

            return false;
        }
    }
}
