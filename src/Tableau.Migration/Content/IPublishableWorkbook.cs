﻿// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Intreface for a <see cref="IWorkbook"/> that has been downloaded
    /// and has full information necessary for re-publishing.
    /// </summary>
    public interface IPublishableWorkbook : IWorkbookDetails, IFileContent, IConnectionsContent
    {
        /// <summary>
        /// Gets the ID of the user to generate thumbnails as.
        /// </summary>
        Guid? ThumbnailsUserId { get; set; }
    }
}
