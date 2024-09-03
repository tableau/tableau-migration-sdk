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
using System.Threading.Tasks;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Content
{

    internal class PublishableCustomView : CustomView, IPublishableCustomView
    {
        public PublishableCustomView(
            ICustomView customView,
            IList<IContentReference> defaultUsers,
            IContentFileHandle file)
            : base(customView)
        {
            DefaultUsers = defaultUsers;
            File = file;
        }

        public IList<IContentReference> DefaultUsers { get; set; }
        public IContentFileHandle File { get; set; }

        #region - IAsyncDisposable Implementation -

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await File.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
