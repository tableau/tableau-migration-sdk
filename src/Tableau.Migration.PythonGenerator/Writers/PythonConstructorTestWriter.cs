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

using Microsoft.CodeAnalysis;
using System.Linq;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonConstructorTestWriter : PythonMemberWriter, IPythonConstructorTestWriter
    {
        public PythonConstructorTestWriter()
        { }

        public void Write(IndentingStringBuilder builder, PythonType type)
        {
            using (var ctorBuilder = builder.AppendLineAndIndent($"def test_ctor(self):"))
            {
                BuildCtorTestBody(type, ctorBuilder);
            }

            builder.AppendLine();
        }

        private static void BuildCtorTestBody(PythonType type, IndentingStringBuilder ctorBuilder)
        {
            var dotnetObj = "dotnet";
            var pyObj = "py";

            if (!type.DotNetType.IsGenericType)
            {
                ctorBuilder.AppendLine($"dotnet = self.create({type.DotNetType.Name})");
                ctorBuilder.AppendLine($"{pyObj} = {type.Name}({dotnetObj})");
            }
            else
            {
                ctorBuilder.AppendLine($"dotnet = self.create({type.DotNetType.OriginalDefinition.Name}[{BuildDotnetGenericTypeConstraintsString(type.DotNetType)}])");
                ctorBuilder.AppendLine($"{pyObj} = {type.Name}[{BuildPythongGenericTypeConstraintsString(type.DotNetType)}]({dotnetObj})");
            }
            
            ctorBuilder.AppendLine($"assert {pyObj}._dotnet == {dotnetObj}");
        }
    }
}
