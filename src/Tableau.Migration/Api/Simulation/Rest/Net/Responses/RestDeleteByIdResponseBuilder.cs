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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestDeleteByIdResponseBuilder<TResponse, TResponseItem> : RestEntityIdResponseBuilderBase<TResponse, TResponseItem>
        where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
        where TResponseItem : IRestIdentifiable
    {
        public RestDeleteByIdResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> deleteData,
            bool requiresAuthentication)
            : base(data, serializer, deleteData, requiresAuthentication)
        { }

        protected override TResponseItem DoServerWork(HttpRequestMessage request, TResponseItem foundEntity, ICollection<TResponseItem> allEntities)
        {
            allEntities.Remove(foundEntity);
            return foundEntity;
        }

        protected override (TResponse Response, HttpStatusCode ResponseCode) BuildEntityResponse(TResponseItem entity)
        {
            return (new TResponse(), HttpStatusCode.NoContent);
        }
    }
}
