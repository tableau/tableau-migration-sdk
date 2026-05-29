//
//  Copyright (c) 2026, Salesforce, Inc.
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

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal class PythonEnumValueWriter : PythonMemberWriter, IPythonEnumValueWriter
    {
        public void Write(IndentingStringBuilder builder, PythonType type, PythonEnumValue enumValue)
        {
            // Enumeration docstrings must use a special form or they don't get picked up correctly by sphinx.
            if (enumValue.Documentation is not null)
            {
                builder.AppendLine("#: " + enumValue.Documentation.Summary);
            }

            string enumValueToken;
            if (enumValue.Value is string)
            {
                enumValueToken = $"\"{enumValue.Value}\"";
            }
            else
            {
                enumValueToken = enumValue.Value.ToString() ?? string.Empty;
            }

            builder.AppendLine($"{enumValue.Name} = {enumValueToken}");
            builder.AppendLine();
        }
    }
}
