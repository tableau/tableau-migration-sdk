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
using Microsoft.Extensions.Logging;

namespace Tableau.Migration.Interop.Logging
{
    /// <summary>
    /// Interface for a logger that does not use a generic state type.
    /// </summary>
    public interface INonGenericLogger
    {
        /// <summary>
        /// Writes a log entry with a pre-formatted state object.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The pre-formatted entry to be written.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="message">The pre-formatted message to write.</param>
        void Log(LogLevel logLevel, EventId eventId, string state, Exception? exception, string message);
    }
}
