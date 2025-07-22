//
//  Copyright (c) 2025, Salesforce, Inc.
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Tableau.Migration.PythonGenerator.Writers
{
    internal sealed class PythonMethodWriter : PythonMemberWriter, IPythonMethodWriter
    {
        private readonly IPythonDocstringWriter _docWriter;

        public PythonMethodWriter(IPythonDocstringWriter docWriter)
        {
            _docWriter = docWriter;
        }

        private string BuildArgumentDeclarations(PythonType type, PythonMethod method)
        {
            string ArgumentDeclaration(PythonMethodArgument arg)
                => $"{arg.Name}: {ToPythonTypeDeclaration(type, arg.Type)}";

            var prefix = method.IsStatic ? "cls" : "self";

            return string.Join(", ", method.Arguments.Select(ArgumentDeclaration).Prepend(prefix));
        }

        private string BuildArgumentInvocation(PythonMethod method)
        {
            string ArgumentInvocation(PythonMethodArgument arg)
            {
                return ToDotNetType(arg.Type, arg.Name);
            }

            var argInvocations = new List<string>(method.DotNetMethod.Parameters.Length);
            var pythonArgQueue = new Queue<PythonMethodArgument>(method.Arguments);
            foreach(var dotNetParam in method.DotNetMethod.Parameters)
            {
                if(dotNetParam.Type.Name == nameof(CancellationToken))
                {
                    argInvocations.Add("cancellation_token");
                }
                else
                {
                    argInvocations.Add(ArgumentInvocation(pythonArgQueue.Dequeue()));
                }
            }

            return string.Join(", ", argInvocations);
        }

        private string UnwrapTaskType(ITypeSymbol type)
        {
            if (type.Name != nameof(Task<int>) || type is not INamedTypeSymbol namedType)
                return DotNetTypeName(type);

            return DotNetTypeName(namedType.TypeArguments.Single());
        }

        private string BuildInvokeExpression(PythonType type, PythonMethod method)
        {
            var argInvocation = BuildArgumentInvocation(method);
            var dotNetInvoker = method.IsStatic ? type.DotNetType.Name : $"self.{PythonTypeWriter.DOTNET_OBJECT}";
            var methodInvokeExpression = $"{dotNetInvoker}.{method.DotNetMethod.Name}({argInvocation})";

            if(method.IsAsync)
            {
                var taskReturn = UnwrapTaskType(method.DotNetMethod.ReturnType);
                methodInvokeExpression = $"TaskExtensions.AwaitResult[{taskReturn}]({methodInvokeExpression})";
            }

            return methodInvokeExpression;
        }

        public void Write(IndentingStringBuilder builder, PythonType type, PythonMethod method)
        {
            var returnTypeName = method.ReturnType is null ? "None" : ToPythonTypeDeclaration(type, method.ReturnType);

            if (method.IsStatic)
            {
                builder.AppendLine("@classmethod");
            }

            var argDeclarations = BuildArgumentDeclarations(type, method);
            using (var methodBuilder = builder.AppendLineAndIndent($"def {method.Name}({argDeclarations}) -> {returnTypeName}:"))
            {
                _docWriter.Write(methodBuilder, method.Documentation);

                var methodInvokeExpression = BuildInvokeExpression(type, method);

                if (method.ReturnType is not null)
                {
                    methodBuilder.AppendLine($"result = {methodInvokeExpression}");
                    var returnExpression = ToPythonType(method.ReturnType, "result");
                    methodBuilder.AppendLine($"return {returnExpression}");
                }
                else
                {
                    methodBuilder.AppendLine(methodInvokeExpression);
                }
            }

            builder.AppendLine();
        }
    }
}
