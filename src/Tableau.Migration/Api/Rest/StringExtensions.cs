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

namespace Tableau.Migration.Api.Rest
{
    internal static class StringExtensions
    {
        public static TValue To<TValue>(this string? value, Func<string?, TValue> convert)
            where TValue : class
            => convert(value);

        public static TValue? To<TValue>(this string? value, Func<string?, TValue?> convert)
            where TValue : struct
            => convert(value);

        public static DateTime? ToDateTimeOrNull(this string? value, bool isIso8601 = true)
            => value.To(
                v =>
                {
                    if (isIso8601)
                    {
                        return v.TryParseFromIso8601(false);
                    }
                    else
                    {
                        if (DateTime.TryParse(v, out var result))
                            return result;
                    }

                    return null;
                });

        public static TimeOnly? ToTimeOrNull(this string? value)
            => value.To<TimeOnly>(
                v =>
                {
                    if (TimeOnly.TryParse(v, out var result))
                        return result;

                    return null;
                });

        public static int? ToIntOrNull(this string? value)
            => value.To<int>(
                v =>
                {
                    if (int.TryParse(v, out var result))
                        return result;

                    return null;
                });

        public static bool? ToBoolOrNull(this string? value)
            => value.To<bool>(
                v =>
                {
                    if (bool.TryParse(v, out var result))
                        return result;

                    return null;
                });
    }
}
