﻿//
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
using System.Text.Json;

namespace Tableau.Migration.JsonConverters
{
    internal static class JsonWriterUtils
    {
        internal static void WriteExceptionProperties(ref Utf8JsonWriter writer, Exception value)
        {
            writer.WriteString("Message", value.Message);
            writer.WriteString("Type", value.GetType().FullName);
            writer.WriteString("Source", value.Source);
            writer.WriteString("StackTrace", value.StackTrace);
            writer.WriteString("HelpLink", value.HelpLink); // Include HelpLink
            writer.WriteNumber("HResult", value.HResult); // Include HResult

            // Leaving this at 1 level of depth.
            if (value.InnerException != null)
            {
                writer.WriteStartObject("InnerException");
                writer.WriteString("Message", value.InnerException.Message);
                writer.WriteString("Type", value.InnerException.GetType().FullName);
                writer.WriteString("Source", value.InnerException.Source);
                writer.WriteString("StackTrace", value.InnerException.StackTrace);
                writer.WriteString("HelpLink", value.InnerException.HelpLink); // Include HelpLink for InnerException
                writer.WriteNumber("HResult", value.InnerException.HResult); // Include HResult for InnerException
                writer.WriteEndObject();
            }
        }
    }
}
