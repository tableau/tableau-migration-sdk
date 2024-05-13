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

using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a file upload response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class FileUploadResponse : TableauServerResponse<FileUploadResponse.FileUploadType>
    {
        /// <summary>
        /// Gets or sets the file upload type for the response.
        /// </summary>
        [XmlElement("fileUpload")]
        public override FileUploadType? Item { get; set; }

        /// <summary>
        /// Creates a new <see cref="FileUploadResponse"/> instance.
        /// </summary>
        public FileUploadResponse()
        {
            //Do not remove, needed for serialization.
        }

        /// <summary>
        /// Creates a new <see cref="FileUploadResponse"/> instance.
        /// </summary>
        /// <param name="uploadSessionId">The server's upload session id.</param>
        /// <param name="fileSize">The uploaded file size.</param>
        internal FileUploadResponse(string uploadSessionId, long fileSize)
        {
            Item = new FileUploadType(uploadSessionId, fileSize);
        }

        /// <summary>
        /// Creates a new <see cref="FileUploadResponse"/> instance.
        /// </summary>
        /// <param name="error">The error for the response</param>
        internal FileUploadResponse(Error error)
            : base(error)
        { }

        /// <summary>
        /// Class representing a file upload response.
        /// </summary>
        public class FileUploadType
        {
            /// <summary>
            /// Gets or sets the upload session id for the response.
            /// </summary>
            [XmlAttribute("uploadSessionId")]
            public string? UploadSessionId { get; set; }

            /// <summary>
            /// Gets or sets the file size for the response.
            /// </summary>
            [XmlAttribute("fileSize")]
            public long FileSize { get; set; }

            /// <summary>
            /// Creates a new <see cref="FileUploadType"/> instance.
            /// </summary>
            public FileUploadType()
            {
                //Do not remove, needed for serialization.
            }

            /// <summary>
            /// Creates a new <see cref="FileUploadType"/> instance.
            /// </summary>
            /// <param name="uploadSessionId">The server's generated session ID.</param>
            /// <param name="fileSize">The server's product version.</param>
            internal FileUploadType(string uploadSessionId, long fileSize)
            {
                UploadSessionId = uploadSessionId;

                FileSize = fileSize;
            }
        }
    }
}
