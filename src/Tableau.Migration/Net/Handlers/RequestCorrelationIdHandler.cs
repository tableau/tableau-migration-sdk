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

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration;

/// <summary>
/// A handler that adds a unique request ID to the HTTP request and response headers.
/// </summary>
public class RequestCorrelationIdHandler : DelegatingHandler
{
    /// <summary>
    /// Sends an HTTP request with a unique request ID and adds the same ID to the response headers.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();
        request.Headers.Add(Constants.REQUEST_CORRELATION_ID_HEADER, correlationId);

        // Start a new activity with the correlation ID
        var activity = new Activity("HttpRequest");
        activity.AddTag(Constants.REQUEST_CORRELATION_ID_HEADER, correlationId);
        activity.Start();

        // Set the current activity
        Activity.Current = activity;

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.Headers.Add(Constants.REQUEST_CORRELATION_ID_HEADER, correlationId);

        // Stop the activity
        activity.Stop();

        return response;
    }
}
