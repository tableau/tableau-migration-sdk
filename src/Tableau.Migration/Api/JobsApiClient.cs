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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal class JobsApiClient : ContentApiClientBase, IJobsApiClient
    {
        private readonly ITaskDelayer _taskDelayer;
        private readonly IConfigReader _configReader;
        private readonly TimeProvider _timeProvider;

        public JobsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ITaskDelayer taskDelayer,
            IConfigReader configReader,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            TimeProvider timeProvider)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _taskDelayer = taskDelayer;
            _configReader = configReader;
            _timeProvider = timeProvider;
        }

        public async Task<IResult<IJob>> GetJobStatusAsync(Guid jobId, CancellationToken cancel)
        {
            var jobResponse = await RestRequestBuilderFactory
                .CreateUri($"/jobs/{jobId.ToUrlSegment()}")
                .ForGetRequest()
                .SendAsync<JobResponse>(cancel)
                .ConfigureAwait(false);

            return jobResponse.ToResult<JobResponse, IJob>(r => new Job(r), SharedResourcesLocalizer);
        }

        private static bool IsErrorStatus(IStatusNote statusNote)
            => string.Equals(statusNote.Type, "errorCode", StringComparison.OrdinalIgnoreCase);

        public async Task<IResult> WaitForJobAsync(Guid jobId, CancellationToken cancel)
        {
            var startTime = _timeProvider.GetUtcNow();
            IJob? job = null;
            while (true)
            {
                cancel.ThrowIfCancellationRequested();

                // Check job waiting timeout
                var timeSinceStart = _timeProvider.GetUtcNow() - startTime;
                if(timeSinceStart > _configReader.Get().Jobs.JobTimeout)
                {
                    return Result.Failed(new TimeoutJobException(job, SharedResourcesLocalizer));
                }

                var jobResult = await GetJobStatusAsync(jobId, cancel).ConfigureAwait(false);

                // See if the job finished in some way.
                if (!jobResult.Success || jobResult.Value.ProgressPercentage >= 100)
                {
                    // Tableau cleans up completed jobs frequently so a job
                    // may have completed and been cleaned up before a single status
                    // check or between polling attempts.
                    //
                    // We try to detect this through looking for a 404 or 400031 error code
                    // to consider that a "success" result.
                    if (!jobResult.Success)
                    {
                        if (jobResult.Errors[0] is RestException restError)
                        {
                            if (string.Equals(restError.Code, "400031", StringComparison.Ordinal) ||
                                restError.Code?.StartsWith("404") == true)
                            {
                                return Result.Succeeded();
                            }
                        }
                    }
                    else
                    {
                        // Detect a failed job with a success HTTP status code.
                        // Tableau may return a failed job even if the 
                        // HTTP status code is a success.
                        //
                        // This might be through a non-zero "finish code"
                        // on the job, or an error code in the status messages
                        // even with a successful "finish code."

                        job = jobResult.Value;
                        if (job.FinishCode != 0 ||
                           job.StatusNotes.Any(IsErrorStatus))
                        {
                            return Result.Failed(new FailedJobException(job, SharedResourcesLocalizer));
                        }
                    }

                    return jobResult;
                }

                await _taskDelayer.DelayAsync(_configReader.Get().Jobs.JobPollRate, cancel).ConfigureAwait(false);
            }
        }
    }
}
