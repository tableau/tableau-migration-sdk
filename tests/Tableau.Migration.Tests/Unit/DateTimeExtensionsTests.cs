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
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class DateTimeExtensionsTests
    {
        #region - Iso8601 -

        //7/14/2016 2:52:31 PM UTC
        private const long TEST_TICKS = 636041047510567945;

        //7/14/2016 2:52:31 PM UTC
        private const string TEST_ISO8601_UTC = "2016-07-14T14:52:31Z";

        //7/14/2016 9:52:31 AM CDT (-5:00)
        private const string TEST_ISO8601_NOT_UTC = "2016-07-14T09:52:31-05:00";

        #region - ToIso8601 -

        public class ToIso8601
        {
            [Theory]
            [CultureNameTestData]
            public void Utc(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var dt = new DateTime(TEST_TICKS, DateTimeKind.Utc);
                    Assert.Equal(TEST_ISO8601_UTC, dt.ToIso8601());
                }
            }

            [Theory]
            [CultureNameTestData]
            public void LocalTime(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var dt = new DateTime(TEST_TICKS, DateTimeKind.Local);

                    Assert.Equal(dt.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.GetCultureInfo("en-US")), dt.ToIso8601());
                }
            }
        }

        #endregion

        #region - ParseFromIso8601 -

        public class ParseFromIso8601
        {
            [Theory]
            [CultureNameTestData]
            public void ParsesRoundtrip(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var dt = new DateTime(TEST_TICKS, DateTimeKind.Utc);
                    var dt2 = dt.ToIso8601().ParseFromIso8601();

                    Assert.Equal(dt.FloorDateTime(TimeSpan.FromSeconds(1)), dt2);
                }
            }

            [Theory]
            [CultureNameTestData]
            public void EnsureUtcFromUtc(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var expectedResult = new DateTime(TEST_TICKS, DateTimeKind.Utc);
                    var result = TEST_ISO8601_UTC.ParseFromIso8601();

                    Assert.Equal(DateTimeKind.Utc, result.Kind);
                    Assert.Equal(expectedResult.FloorDateTime(TimeSpan.FromSeconds(1)), result.FloorDateTime(TimeSpan.FromSeconds(1)));
                }
            }

            [Theory]
            [CultureNameTestData]
            public void EnsureUtcFromNotUtc(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var expectedResult = new DateTime(TEST_TICKS, DateTimeKind.Utc);
                    var result = TEST_ISO8601_NOT_UTC.ParseFromIso8601();

                    Assert.Equal(DateTimeKind.Utc, result.Kind);
                    Assert.Equal(expectedResult.FloorDateTime(TimeSpan.FromSeconds(1)), result.FloorDateTime(TimeSpan.FromSeconds(1)));
                }
            }

            [Theory]
            [CultureNameTestData]
            public void ExplicitlyDoNotEnsureUtcFromNotUtc(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var result = TEST_ISO8601_NOT_UTC.ParseFromIso8601(false);

                    Assert.NotEqual(DateTimeKind.Utc, result.Kind);
                }
            }
        }

        #endregion

        #region - TryParseFromIso8601 -

        public class TryParseFromIso8601
        {
            [Theory]
            [CultureNameTestData]
            public void ParsesRoundtrip(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var dt = new DateTime(TEST_TICKS, DateTimeKind.Utc);

                    var s = dt.ToIso8601();

                    var parsed = s.TryParseFromIso8601(out DateTime dt2);

                    Assert.Equal(dt.FloorDateTime(TimeSpan.FromSeconds(1)), dt2);
                }
            }

            [Theory]
            [CultureNameTestData]
            public void SuccessfulParse(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var result = TEST_ISO8601_UTC.TryParseFromIso8601(out DateTime dateTime);

                    Assert.True(result);
                    Assert.NotEqual(default(DateTime), dateTime);
                }
            }

            [Theory]
            [CultureNameTestData]
            public void FailedParse(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var result = "7/14/2016 2:52:31 PM UTC".TryParseFromIso8601(out DateTime dateTime);

                    Assert.False(result);
                    Assert.Equal(default(DateTime), dateTime);
                }
            }

            [Theory]
            [CultureNameTestData]
            public void EnsureUtcFromUtc(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var expectedResult = new DateTime(TEST_TICKS, DateTimeKind.Utc);
                    var conversionSuccess = TEST_ISO8601_UTC.TryParseFromIso8601(out DateTime result);

                    Assert.True(conversionSuccess);
                    Assert.Equal(DateTimeKind.Utc, result.Kind);
                    Assert.Equal(expectedResult.FloorDateTime(TimeSpan.FromSeconds(1)), result.FloorDateTime(TimeSpan.FromSeconds(1)));
                }
            }

            [Theory]
            [CultureNameTestData]
            public void EnsureUtcFromNotUtc(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var expectedResult = new DateTime(TEST_TICKS, DateTimeKind.Utc);
                    var conversionSuccess = TEST_ISO8601_NOT_UTC.TryParseFromIso8601(out DateTime result);

                    Assert.True(conversionSuccess);
                    Assert.Equal(DateTimeKind.Utc, result.Kind);
                    Assert.Equal(expectedResult.FloorDateTime(TimeSpan.FromSeconds(1)), result.FloorDateTime(TimeSpan.FromSeconds(1)));
                }
            }

            [Theory]
            [CultureNameTestData]
            public void ExplicitlyDoNotEnsureUtcFromNotUtc(string cultureName)
            {
                using (CultureSilo.FromName(cultureName))
                {
                    var conversionSuccess = TEST_ISO8601_NOT_UTC.TryParseFromIso8601(out DateTime result, false);

                    Assert.True(conversionSuccess);
                    Assert.NotEqual(DateTimeKind.Utc, result.Kind);
                }
            }
        }

        #endregion

        #endregion

        #region - EnsureUtcKind -

        public class EnsureUtcKind
        {
            [Fact]
            public void NullableUtcKind()
            {
                DateTime? d = DateTime.UtcNow;
                DateTime? ud = d.EnsureUtcKind();

                Assert.Equal(d, ud);
            }

            [Fact]
            public void NullableUnspecifiedKind()
            {
                DateTime? d = new DateTime(5700, DateTimeKind.Unspecified);
                DateTime? ud = d.EnsureUtcKind();

                Assert.Equal(d.Value.Ticks, ud?.Ticks);
                Assert.Equal(DateTimeKind.Utc, ud?.Kind);
            }

            [Fact]
            public void NullReturnsNull()
            {
                DateTime? d = null;
                DateTime? ud = d.EnsureUtcKind();

                Assert.Null(ud);
            }
        }

        #endregion
    }
}
