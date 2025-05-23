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
using System.Diagnostics;
using System.Net.Http;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// Class representing an error from a Tableau REST API
    /// </summary>
    public class RestException : Exception, IEquatable<RestException>
    {
        /// <summary>
        /// Gets the request URI from Tableau API.
        /// </summary>
        public readonly HttpMethod? HttpMethod;

        /// <summary>
        /// Gets the request URI from Tableau API.
        /// </summary>
        public readonly Uri? RequestUri;

        /// <summary>
        /// Gets the error code from Tableau API.
        /// </summary>
        public readonly string? Code;

        /// <summary>
        /// Gets the error detail from Tableau API.
        /// </summary>
        public readonly string? Detail;

        /// <summary>
        /// Gets the error summary from Tableau API.
        /// </summary>
        public readonly string? Summary;

        /// <summary>
        /// Gets the request Correlation ID
        /// </summary>
        public readonly string? CorrelationId;

        /// <summary>
        /// Gets the full stack trace. Added for easier debuggability.
        /// </summary>
        public readonly string? FullStackTrace;

        /// <summary>
        /// Creates a new <see cref="RestException"/> instance.
        /// </summary>
        /// <remarks>This should only be used for deserialization.</remarks>
        /// <param name="httpMethod">The http method that generated the current error.</param>
        /// <param name="requestUri">The request URI that generated the current error.</param>
        /// <param name="correlationId">The request Correlation ID</param>
        /// <param name="error">The <see cref="Error"/> returned from the Tableau API.</param>
        /// <param name="fullStackTrace">The stack trace of the caller. Added for easier debuggability.</param>
        /// <param name="exceptionMessage">Message for base Exception.</param>
        internal RestException(
            HttpMethod? httpMethod,
            Uri? requestUri,
            string? correlationId,
            Error error,
            string? fullStackTrace,
            string exceptionMessage)
            : base(exceptionMessage)
        {
            HttpMethod = httpMethod;
            RequestUri = requestUri;
            CorrelationId = correlationId;
            Code = error.Code;
            Detail = error.Detail;
            Summary = error.Summary;
            FullStackTrace = fullStackTrace ?? new StackTrace(fNeedFileInfo: true).ToString();
        }

        /// <summary>
        /// Creates a new <see cref="RestException"/> instance.
        /// </summary>
        /// <param name="httpMethod">The http method that generated the current error.</param>
        /// <param name="requestUri">The request URI that generated the current error.</param>
        /// <param name="correlationId">The request Correlation ID</param>
        /// <param name="error">The <see cref="Error"/> returned from the Tableau API.</param>
        /// <param name="fullStackTrace">The stack trace of the caller. Added for easier debuggability.</param>
        /// <param name="sharedResourcesLocalizer">A string localizer.</param>
        public RestException(
            HttpMethod? httpMethod,
            Uri? requestUri,
            string? correlationId,
            Error error,
            string? fullStackTrace,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : this(httpMethod, requestUri, correlationId, error, fullStackTrace, FormatError(httpMethod, requestUri, correlationId, error, sharedResourcesLocalizer))
        { }

        private static string FormatError(
            HttpMethod? httpMethod,
            Uri? requestUri,
            string? correlationId,
            Error error,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
        {
            var nullValue = $"<{sharedResourcesLocalizer[SharedResourceKeys.NullValue]}>";

            return string.Format(
                sharedResourcesLocalizer[SharedResourceKeys.RestExceptionContent],
                httpMethod,
                requestUri,
                correlationId,
                error.Code ?? nullValue,
                error.Summary ?? nullValue,
                error.Detail ?? nullValue);
        }

        #region - IEquatable - 

        /// <inheritdoc/>
        public bool Equals(RestException? other)
        {
            if (other == null) return false;

            return HttpMethod == other.HttpMethod &&
                   RequestUri == other.RequestUri &&
                   Code == other.Code &&
                   Detail == other.Detail &&
                   Summary == other.Summary &&
                   FullStackTrace == other.FullStackTrace;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is RestException other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(HttpMethod, RequestUri, Code, Detail, Summary, FullStackTrace);
        }

        #endregion

    }
}
