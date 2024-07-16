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
using System.Text.Json;
using System.Text.Json.Serialization;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.JsonConverters
{
    /// <summary>
    /// JsonConverter that serializes a <see cref="Job"/>.
    /// </summary>
    internal class JobJsonConverter : JsonConverter<IJob>
    {
        public JobJsonConverter()
        { }

        public override void Write(Utf8JsonWriter writer, IJob value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(IJob.Id), value.Id);
            writer.WriteString(nameof(IJob.Type), value.Type);
            writer.WriteString(nameof(IJob.CreatedAtUtc), value.CreatedAtUtc);

            if (value.UpdatedAtUtc.HasValue)
            {
                writer.WriteString(nameof(IJob.UpdatedAtUtc), value.UpdatedAtUtc.Value);
            }

            if (value.CompletedAtUtc.HasValue)
            {
                writer.WriteString(nameof(IJob.CompletedAtUtc), value.CompletedAtUtc.Value);
            }

            writer.WriteNumber(nameof(IJob.ProgressPercentage), value.ProgressPercentage);
            writer.WriteNumber(nameof(IJob.FinishCode), value.FinishCode);

            // Serializing StatusNotes
            writer.WriteStartArray(nameof(IJob.StatusNotes));
            foreach (var statusNote in value.StatusNotes)
            {
                writer.WriteStartObject();
                writer.WriteString(nameof(IStatusNote.Type), statusNote.Type);
                writer.WriteString(nameof(IStatusNote.Value), statusNote.Value);
                writer.WriteString(nameof(IStatusNote.Text), statusNote.Text);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }



        public override IJob? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }

            var jobResponse = new JobResponse.JobType();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break; // End of the job object.
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read(); // Move to the property value.

                    switch (propertyName)
                    {
                        case nameof(IJob.Id):
                            jobResponse.Id = reader.GetGuid();
                            break;
                        case nameof(IJob.Type):
                            jobResponse.Type = reader.GetString();
                            break;
                        case nameof(IJob.CreatedAtUtc):
                            jobResponse.CreatedAt = reader.GetDateTime().ToString("o");
                            break;
                        case nameof(IJob.UpdatedAtUtc):
                            jobResponse.UpdatedAt = reader.GetDateTime().ToString("o");
                            break;
                        case nameof(IJob.CompletedAtUtc):
                            jobResponse.CompletedAt = reader.GetDateTime().ToString("o");
                            break;
                        case nameof(IJob.ProgressPercentage):
                            jobResponse.Progress = reader.GetInt32();
                            break;
                        case nameof(IJob.FinishCode):
                            jobResponse.FinishCode = reader.GetInt32();
                            break;
                        case nameof(IJob.StatusNotes):
                            jobResponse.StatusNotes = ReadStatusNotes(ref reader);
                            break;
                        default:
                            reader.Skip(); // Skip unknown properties.
                            break;
                    }
                }
            }

            var jobResponseWrapper = new JobResponse { Item = jobResponse };
            return new Job(jobResponseWrapper);
        }

        private JobResponse.JobType.StatusNoteType[] ReadStatusNotes(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token for StatusNotes");
            }

            var statusNotes = new List<JobResponse.JobType.StatusNoteType>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var statusNote = new JobResponse.JobType.StatusNoteType();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            var propertyName = reader.GetString();
                            reader.Read(); // Move to the property value.

                            switch (propertyName)
                            {
                                case nameof(IStatusNote.Type):
                                    statusNote.Type = reader.GetString();
                                    break;
                                case nameof(IStatusNote.Value):
                                    statusNote.Value = reader.GetString();
                                    break;
                                case nameof(IStatusNote.Text):
                                    statusNote.Text = reader.GetString();
                                    break;
                                default:
                                    reader.Skip(); // Skip unknown properties.
                                    break;
                            }
                        }
                    }
                    statusNotes.Add(statusNote);
                }
            }

            return statusNotes.ToArray();
        }
    }
}
