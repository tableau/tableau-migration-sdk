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

using System.Linq;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal class PythonClassTestWriter : IPythonClassTestWriter
    {
        private readonly IPythonPropertyTestWriter _propertyTestWriter;        
        private readonly IPythonConstructorTestWriter _constructorTestWriter;

        public PythonClassTestWriter(
            IPythonConstructorTestWriter constructorTestWriter,
            IPythonPropertyTestWriter propertyTestWriter)
        {
            _propertyTestWriter = propertyTestWriter;            
            _constructorTestWriter = constructorTestWriter;
        }
        
        public void Write(IndentingStringBuilder builder, PythonType type)
        {
            if (!type.Properties.Any() & !type.Methods.Any())
            {
                return;
            }

            using var classBuilder = builder.AppendLineAndIndent($"class Test{type.Name}Generated(AutoFixtureTestBase):");
            classBuilder.AppendLine();

            if (!type.EnumValues.Any())
            {              
                _constructorTestWriter.Write(classBuilder, type);

                foreach (var property in type.Properties)
                {
                    _propertyTestWriter.Write(classBuilder, type, property);
                }
            }
        }
    }
}
