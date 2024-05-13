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

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Network.Resilience options related to Tableau connections.
    /// </summary>
    public class ResilienceOptions
    {
        /// <summary>
        /// Defaults for <see cref="NetworkOptions.Resilience"/> Settings.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default Retry Flag. Enabled as Default.
            /// </summary>
            public const bool RETRY_ENABLED = true;

            /// <summary>
            /// The default retry intervals for error responses.
            /// 5 retries (0.5 sec, 0.5 sec, 0.5 sec, 1 sec, 2 secs).
            /// </summary>
            public readonly static TimeSpan[] RETRY_INTERVALS = new TimeSpan[]
            {
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2)
            };

            /// <summary>
            /// The default Retry Override Response Codes. Empty by Default.
            /// </summary>
            public readonly static int[] RETRY_OVERRIDE_RESPONSE_CODES = Array.Empty<int>();

            /// <summary>
            /// The default Concurrent Requests Limit Flag. Disabled by Default.
            /// </summary>
            public const bool CONCURRENT_REQUESTS_LIMIT_ENABLED = false;

            /// <summary>
            /// The default Maximum Concurrent Requests. Default is Processor count / 2
            /// </summary>
            public readonly static int MAX_CONCURRENT_REQUESTS = Environment.ProcessorCount / 2;

            /// <summary>
            /// The default Concurrent Waiting Requests on Queue. Default is Processor count / 4
            /// </summary>
            public readonly static int CONCURRENT_WAITING_REQUESTS_QUEUE = Environment.ProcessorCount / 4;

            /// <summary>
            /// The default Requests Client Throttle (Rate-Limit) Flag. Disabled as Default.
            /// </summary>
            public const bool CLIENT_THROTTLE_ENABLED = false;

            /// <summary>
            /// The default server throttle enabled value. Enabled by default.
            /// </summary>
            public const bool SERVER_THROTTLE_ENABLED = true;

            /// <summary>
            /// The default value on whether to limit server throttle retries. False by default.
            /// </summary>
            public const bool SERVER_THROTTLE_LIMIT_RETRIES = false;

            /// <summary>
            /// The default retry intervals for server throttling.
            /// 5 intervals (1 sec, 3 sec, 10 sec, 30 sec, 1 minute).
            /// </summary>
            public readonly static TimeSpan[] SERVER_THROTTLE_RETRY_INTERVALS = new TimeSpan[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(1)
            };

            /// <summary>
            /// The default Maximum Read Requests for the Client Throttle. Default is 40000.
            /// </summary>
            public const int MAX_READ_REQUESTS = 40000;

            /// <summary>
            /// The default interval for Read Requests Throttle. Default is 1 hour
            /// </summary>
            public readonly static TimeSpan MAX_READ_REQUESTS_INTERVAL = TimeSpan.FromHours(1);

            /// <summary>
            /// The default Maximum Publish Requests for the Client Throttle. Default is 5500.
            /// </summary>
            public const int MAX_PUBLISH_REQUESTS = 5500;

            /// <summary>
            /// The default interval for Publish Requests Throttle.
            /// </summary>
            public readonly static TimeSpan MAX_PUBLISH_REQUESTS_INTERVAL = TimeSpan.FromDays(1);

            /// <summary>
            /// The default Per-Request Timeout. Default is 30 minutes.
            /// </summary>
            public readonly static TimeSpan REQUEST_TIMEOUT = TimeSpan.FromMinutes(30);

            /// <summary>
            /// The default Per-FileTransferRequest Timeout. Default is 12 hours.
            /// </summary>
            public readonly static TimeSpan FILE_TRANSFER_REQUEST_TIMEOUT = TimeSpan.FromHours(12);
        }

        /// <summary>
        /// Indicates if we retry requests in case of errors or not.
        /// </summary>
        public bool RetryEnabled
        {
            get => _retryEnabled ?? Defaults.RETRY_ENABLED;
            set => _retryEnabled = value;
        }
        private bool? _retryEnabled;

        /// <summary>
        /// Indicates the intervals between each retry. 
        /// The default value is 5 retries (0.5 sec, 0.5 sec, 0.5 sec, 1 sec, 2 secs).
        /// This configuration depends on <see cref="RetryEnabled"/> and <see cref="RetryOverrideResponseCodes"/>.
        /// </summary>
        public TimeSpan[] RetryIntervals
        {
            get => _retryIntervals ?? Defaults.RETRY_INTERVALS;
            set => _retryIntervals = value;
        }
        private TimeSpan[]? _retryIntervals;

        /// <summary>
        /// Indicates the overridden response codes that will be retried. This configuration is empty as default.
        /// The default System.Net.HttpStatusCodes configured to be handled are:
        /// • HTTP 5XX status codes(server errors)
        /// • HTTP 408 status code(request timeout)
        /// In the case you want to retry only to a specific response code, set this configuration override.
        /// This configuration depends on <see cref="RetryEnabled"/> and <see cref="RetryIntervals"/>.
        /// </summary>
        public int[] RetryOverrideResponseCodes
        {
            get => _retryOverrideResponseCodes ?? Defaults.RETRY_OVERRIDE_RESPONSE_CODES;
            set => _retryOverrideResponseCodes = value;
        }
        private int[]? _retryOverrideResponseCodes;

        /// <summary>
        /// Indicates if we limit the number of concurrent requests or not. The default value is `disabled`.
        /// </summary>
        public bool ConcurrentRequestsLimitEnabled
        {
            get => _concurrentRequestsLimitEnabled ?? Defaults.CONCURRENT_REQUESTS_LIMIT_ENABLED;
            set => _concurrentRequestsLimitEnabled = value;
        }
        private bool? _concurrentRequestsLimitEnabled;

        /// <summary>
        /// Limits the amount of global concurrent requests at any given time. 
        /// The default value is  
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.environment.processorcount#remarks">
        /// (the number of logical processors)
        /// </see>/2.
        /// </summary>
        public int MaxConcurrentRequests
        {
            get => _maxConcurrentRequests ?? Defaults.MAX_CONCURRENT_REQUESTS;
            set => _maxConcurrentRequests = value;
        }
        private int? _maxConcurrentRequests;

        /// <summary>
        /// Indicates the amount of global concurrent request that will be waiting on the queue for a free concurrent slot. 
        /// The default value is 
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.environment.processorcount#remarks">
        /// (the number of logical processors)
        /// </see>/4.
        /// </summary>
        public int ConcurrentWaitingRequestsOnQueue
        {
            get => _concurrentWaitingRequestsOnQueue ?? Defaults.CONCURRENT_WAITING_REQUESTS_QUEUE;
            set => _concurrentWaitingRequestsOnQueue = value;
        }
        private int? _concurrentWaitingRequestsOnQueue;

        /// <summary>
        /// Indicates if we limit the number requests to a given endpoint or not. 
        /// The default value is `disabled`.
        /// </summary>
        public bool ClientThrottleEnabled
        {
            get => _clientThrottleEnabled ?? Defaults.CLIENT_THROTTLE_ENABLED;
            set => _clientThrottleEnabled = value;
        }
        private bool? _clientThrottleEnabled;

        /// <summary>
        /// Limits the amount of read requests in for the Client Throttle. 
        /// The default value is 40000.
        /// </summary>
        public int MaxReadRequests
        {
            get => _maxReadRequests ?? Defaults.MAX_READ_REQUESTS;
            set => _maxReadRequests = value;
        }
        private int? _maxReadRequests;

        /// <summary>
        /// Indicates the period for the Read Request Throttle. 
        /// The default value is one hour.
        /// </summary>
        public TimeSpan MaxReadRequestsInterval
        {
            get => _maxReadRequestsInterval ?? Defaults.MAX_READ_REQUESTS_INTERVAL;
            set => _maxReadRequestsInterval = value;
        }
        private TimeSpan? _maxReadRequestsInterval;

        /// <summary>
        /// Limits the amount of push requests in for the Client Throttle. 
        /// The default value is 5500.
        /// </summary>
        public int MaxPublishRequests
        {
            get => _maxPushRequests ?? Defaults.MAX_PUBLISH_REQUESTS;
            set => _maxPushRequests = value;
        }
        private int? _maxPushRequests;

        /// <summary>
        /// Indicates the period for the Push Request Throttle. 
        /// The default value is one day.
        /// </summary>
        public TimeSpan MaxPublishRequestsInterval
        {
            get => _maxPushRequestsInterval ?? Defaults.MAX_PUBLISH_REQUESTS_INTERVAL;
            set => _maxPushRequestsInterval = value;
        }
        private TimeSpan? _maxPushRequestsInterval;

        /// <summary>
        /// Gets or sets whether to wait and retry on server throttle responses.
        /// The default value is enabled.
        /// There is not limit to the number of retries from server throttle responses.
        /// <see cref="ServerThrottleRetryIntervals"/> is used to determine the length of time between retries.
        /// If no Retry-After header is supplied by the server, 
        /// with the last retry interval used if the retry count exceeds the retry interval count.
        /// </summary>
        public bool ServerThrottleEnabled
        {
            get => _serverThrottleEnabled ?? Defaults.SERVER_THROTTLE_ENABLED;
            set => _serverThrottleEnabled = value;
        }
        private bool? _serverThrottleEnabled;

        /// <summary>
        /// Gets or sets whether to limit the number of retries on server throttle responses.
        /// The default value is false.
        /// If true the last retry interval of <see cref="ServerThrottleRetryIntervals"/> is not re-used.
        /// </summary>
        public bool ServerThrottleLimitRetries
        {
            get => _serverThrottleLimitRetries ?? Defaults.SERVER_THROTTLE_LIMIT_RETRIES;
            set => _serverThrottleLimitRetries = value;
        }
        private bool? _serverThrottleLimitRetries;

        /// <summary>
        /// Indicates the intervals between each retry during server throttling. 
        /// This value is only used when the server does not supply a Retry-After header.
        /// If <see cref="ServerThrottleLimitRetries"/> is false (the default)
        /// the last retry interval is used if the retry count exceeds the retry interval count.
        /// </summary>
        public TimeSpan[] ServerThrottleRetryIntervals
        {
            get => _serverThrottleRetryIntervals ?? Defaults.SERVER_THROTTLE_RETRY_INTERVALS;
            set => _serverThrottleRetryIntervals = value;
        }
        private TimeSpan[]? _serverThrottleRetryIntervals;

        /// <summary>
        /// Indicates the Per-Request Timeout. The default value is 30 minutes.
        /// </summary>
        public TimeSpan PerRequestTimeout
        {
            get => _requestTimeout ?? Defaults.REQUEST_TIMEOUT;
            set => _requestTimeout = value;
        }
        private TimeSpan? _requestTimeout;

        /// <summary>
        /// Indicates the Per-FileTransferRequest Timeout. The default value is 12 hours.
        /// </summary>
        public TimeSpan PerFileTransferRequestTimeout
        {
            get => _fileTransferRequestTimeout ?? Defaults.FILE_TRANSFER_REQUEST_TIMEOUT;
            set => _fileTransferRequestTimeout = value;
        }
        private TimeSpan? _fileTransferRequestTimeout;
    }
}