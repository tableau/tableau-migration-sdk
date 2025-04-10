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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content
{
    internal abstract class SubscriptionBase<TSchedule> : ContentBase, ISubscription<TSchedule>
        where TSchedule : ISchedule
    {
        /// <inheritdoc/>
        public string Subject { get; set; }

        /// <inheritdoc/>
        public bool AttachImage { get; set; }

        /// <inheritdoc/>
        public bool AttachPdf { get; set; }

        /// <inheritdoc/>
        public string PageOrientation { get; set; }

        /// <inheritdoc/>
        public string PageSizeOption { get; set; }

        /// <inheritdoc/>
        public bool Suspended { get; set; }

        /// <inheritdoc/>
        public string Message { get; set; }

        /// <inheritdoc/>
        public ISubscriptionContent Content { get; set; }

        /// <inheritdoc/>
        public IContentReference Owner { get; set; }

        /// <inheritdoc/>
        public TSchedule Schedule { get; }

        internal SubscriptionBase(Guid id, string? subject, bool attachImage, bool attachPdf,
            string? pageOrientation, string? pageSizeOption, bool suspended, string? message,
            ISubscriptionContent content, IContentReference user, TSchedule schedule)
        {
            Id = Guard.AgainstDefaultValue(id, () => id);
            Subject = Guard.AgainstNull(subject, () => subject);
            AttachImage = attachImage;
            AttachPdf = attachPdf;
            PageOrientation = pageOrientation ?? string.Empty;
            PageSizeOption = pageSizeOption ?? string.Empty;
            Suspended = suspended;
            Message = message ?? string.Empty;

            Content = content;
            Owner = user;
            Schedule = schedule;

            Name = Id.ToString();
            Location = new(Name);
        }

        internal SubscriptionBase(ISubscriptionType response, IContentReference user, TSchedule schedule)
            : this(response.Id, response.Subject, response.AttachImage, response.AttachPdf,
                  response.PageOrientation, response.PageSizeOption, response.Suspended, response.Message,
                  new SubscriptionContent(response.Content), user, schedule)
        { }

        internal SubscriptionBase(ISubscription<TSchedule> response, IContentReference user, TSchedule schedule)
            : this(response.Id, response.Subject, response.AttachImage, response.AttachPdf,
                  response.PageOrientation, response.PageSizeOption, response.Suspended, response.Message,
                  new SubscriptionContent(response.Content), user, schedule)
        { }

        protected static async Task<IImmutableList<TSubscription>> CreateManyAsync<TResponse, TSubscriptionType, TSubscription>(
            TResponse response,
            Func<TResponse, IEnumerable<TSubscriptionType?>> responseItemFactory,
            Func<TSubscriptionType, IContentReference, CancellationToken, Task<TSubscription>> modelFactory,
            IContentReferenceFinderFactory finderFactory,
            ILogger logger, ISharedResourcesLocalizer localizer,
            CancellationToken cancel)
            where TResponse : ITableauServerResponse
            where TSubscriptionType : class, ISubscriptionType
            where TSubscription : ISubscription<TSchedule>
        {
            var items = responseItemFactory(response).ExceptNulls().ToImmutableArray();
            var results = ImmutableArray.CreateBuilder<TSubscription>(items.Length);

            foreach (var item in items)
            {

                var user = await finderFactory.FindUserAsync(Guard.AgainstNull(item.User, () => item.User), logger, localizer, true, cancel).ConfigureAwait(false);

                results.Add(await modelFactory(item, user, cancel).ConfigureAwait(false));
            }

            return results.ToImmutable();
        }
    }
}