﻿//
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

using System;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    internal static class IContentReferenceExtensions
    {
        internal static IContentReference ThrowOnMissingContentReference(this IResult<IContentReference> result, string topExceptionMessage)
        {
            if(result.Success)
            {
                return result.Value;
            }

            throw new AggregateException(topExceptionMessage, result.Errors);
        }

        internal static IContentReference ThrowOnMissingContentReference(this IContentReference? result, string exceptionMessage)
        {
            if(result is not null)
            {
                return result;
            }

            throw new Exception(exceptionMessage);
        }
    }
}
