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
using System.Collections.Generic;
using System.Net.Http;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    /// <summary>
    /// Abstract base calss for REST API style response builders that operate on a list of entities in some form.
    /// </summary>
    internal abstract class RestEntityListResponseBuilderBase<TResponse, TItem> : RestResponseBuilderBase<TResponse>
        where TResponse : TableauServerResponse, new()
    {
        private readonly Func<TableauData, HttpRequestMessage, ICollection<TItem>> _getEntities;

        public RestEntityListResponseBuilderBase(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TItem>> getEntities,
            bool requiresAuthentication)
            : base(data, serializer, requiresAuthentication)
        {
            _getEntities = getEntities;
        }

        protected ICollection<TItem> GetEntities(TableauData data, HttpRequestMessage request)
            => _getEntities(data, request);
    }
}
