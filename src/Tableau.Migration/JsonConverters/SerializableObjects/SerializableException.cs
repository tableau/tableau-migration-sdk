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

namespace Tableau.Migration.JsonConverters.SerializableObjects
{
    /// <summary>
    /// Represents a serializable version of an exception, allowing exceptions to be serialized into JSON format.
    /// </summary>
    public class SerializableException
    {
        /// <summary>
        /// Gets or sets the class name of the exception.
        /// </summary>
        public string? ClassName { get; set; }

        /// <summary>
        /// Gets or sets the exception object. This property is not serialized and is used only for internal purposes.
        /// </summary>
        public Exception? Error { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableException"/> class using the specified exception.
        /// </summary>
        /// <param name="ex">The exception to serialize.</param>
        internal SerializableException(Exception ex)
        {
            ClassName = ex.GetType().FullName;
            Error = ex;
        }
    }
}
