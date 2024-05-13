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
using System.Globalization;

namespace Tableau.Migration
{
    /// <summary>
    /// Static class containing extension methods for <see cref="DateTime"/> objects.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the ISO-8601 <see cref="DateTime"/> string,
        /// preserves current <see cref="DateTimeKind"/> setting but truncates fractional seconds.
        /// </summary>
        public static string ToIso8601(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a <see cref="DateTime"/> from a string that is in ISO-8601 format.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="ensureUtc">When true, the parsed <see cref="DateTime"/> will be <see cref="DateTimeKind.Utc"/></param>
        /// <returns>The parsed <see cref="DateTime"/> value.</returns>
        public static DateTime ParseFromIso8601(this string value, bool ensureUtc = true)
        {
            var d = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            if (ensureUtc)
            {
                return d.EnsureUtcKind();
            }

            return d;
        }

        /// <summary>
        /// Parses a <see cref="Nullable{DateTime}"/> from a string that is in ISO-8601 format.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="ensureUtc">When true, the parsed <see cref="DateTime"/> will be <see cref="DateTimeKind.Utc"/></param>
        /// <returns>The parsed <see cref="DateTime"/> value, or null if parsing failed.</returns>
        public static DateTime? TryParseFromIso8601(this string? value, bool ensureUtc = true)
        {
            if (value is null)
            {
                return null;
            }

            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime d))
            {
                return null;
            }

            if (ensureUtc)
            {
                d = d.EnsureUtcKind();
            }

            return d;
        }

        /// <summary>
        /// Attempts to parse a <see cref="DateTime"/> from a string that is in ISO-8601 format.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="d">The <see cref="DateTime"/> to assign with the parsed value.</param>
        /// <param name="ensureUtc">When true, the parsed <see cref="DateTime"/> will be <see cref="DateTimeKind.Utc"/></param>
        /// <returns>True if the value was successfully parsed, otherwise false.</returns>
        public static bool TryParseFromIso8601(this string? value, out DateTime d, bool ensureUtc = true)
        {
            var result = TryParseFromIso8601(value, ensureUtc);
            d = result.GetValueOrDefault();
            return result.HasValue;
        }

        /// <summary>
        /// Ensures the <paramref name="value"/> has the <see cref="DateTime.Kind"/> set to <see cref="DateTimeKind.Utc"/>.
        /// Converts the <see cref="DateTime"/> from local time to UTC if needed.
        /// </summary>
        public static DateTime? EnsureUtcKind(this DateTime? value)
        {
            return value.HasValue ? EnsureUtcKind(value.Value) : null;
        }

        /// <summary>
        /// Ensures the <paramref name="value"/> has the <see cref="DateTime.Kind"/> set to <see cref="DateTimeKind.Utc"/>.
        /// Converts the <see cref="DateTime"/> from local time to UTC if needed.
        /// </summary>
        public static DateTime EnsureUtcKind(this DateTime value)
        {
            // Assume UTC if unspecified because this is how we store them in the database.
            if (value.Kind == DateTimeKind.Unspecified)
            {
                value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }

            // Convert local times to UTC if needed.
            if (value.Kind == DateTimeKind.Local)
            {
                value = TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Local, TimeZoneInfo.Utc);
            }

            return value;
        }

        /// <summary>
        /// Lowers the date time to the nearest interval
        /// </summary>
        /// <param name="dt"><see cref="DateTime"/> to lower</param>
        /// <param name="interval">Interval to which to lower</param>
        /// <returns>Lowered date time</returns>
        public static DateTime FloorDateTime(this DateTime dt, TimeSpan interval)
        {
            return dt.AddTicks(-(dt.Ticks % interval.Ticks));
        }

        /// <summary>
        /// Raises the date time to the nearest interval
        /// </summary>
        /// <param name="dt"><see cref="DateTime"/> to raise</param>
        /// <param name="interval">Interval to which to lower</param>
        /// <returns>Raised date time</returns>
        public static DateTime CeilingDateTime(this DateTime dt, TimeSpan interval)
        {
            var overflow = dt.Ticks % interval.Ticks;
            return overflow == 0 ? dt : dt.AddTicks(interval.Ticks - overflow);
        }
    }
}
