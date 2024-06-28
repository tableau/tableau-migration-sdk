# Copyright (c) 2024, Salesforce, Inc.
# SPDX-License-Identifier: Apache-2
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

# region _generated

from datetime import time # noqa: E402, F401
from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration import (  # noqa: E402, F401
    _generic_wrapper,
    PyContentReference
)
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar,
    List
)

from System import TimeOnly # noqa: E402, F401
from System.Collections.Generic import List as DotnetList # noqa: E402, F401
from Tableau.Migration.Content.Schedules import (  # noqa: E402, F401
    IExtractRefreshTask,
    IFrequencyDetails,
    IInterval,
    ISchedule,
    IWithSchedule
)

from tableau_migration.migration_content_schedules import (  # noqa: E402, F401
    PyExtractRefreshContentType,
    PyExtractRefreshTask,
    PyFrequencyDetails,
    PyInterval,
    PySchedule,
    PyWithSchedule
)


from Tableau.Migration.Content.Schedules import ExtractRefreshContentType

# Extra imports for tests.
from Tableau.Migration import IContentReference # noqa: E402, F401
from System import (  # noqa: E402, F401
    Nullable,
    TimeOnly,
    String
)
from System.Collections.Generic import List as DotnetList # noqa: E402, F401
from Tableau.Migration.Content.Schedules import ExtractRefreshContentType # noqa: E402, F401
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyExtractRefreshTaskGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IExtractRefreshTask[ISchedule])
        py = PyExtractRefreshTask[PySchedule](dotnet)
        assert py._dotnet == dotnet
    
    def test_type_getter(self):
        dotnet = self.create(IExtractRefreshTask[ISchedule])
        py = PyExtractRefreshTask[PySchedule](dotnet)
        assert py.type == dotnet.Type
    
    def test_type_setter(self):
        dotnet = self.create(IExtractRefreshTask[ISchedule])
        py = PyExtractRefreshTask[PySchedule](dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.type = testValue
        
        # assert value
        assert py.type == testValue
    
    def test_content_type_getter(self):
        dotnet = self.create(IExtractRefreshTask[ISchedule])
        py = PyExtractRefreshTask[PySchedule](dotnet)
        assert py.content_type.value == (None if dotnet.ContentType is None else PyExtractRefreshContentType(dotnet.ContentType.value__)).value
    
    def test_content_type_setter(self):
        dotnet = self.create(IExtractRefreshTask[ISchedule])
        py = PyExtractRefreshTask[PySchedule](dotnet)
        
        # create test data
        testValue = self.create(ExtractRefreshContentType)
        
        # set property to new test value
        py.content_type = None if testValue is None else PyExtractRefreshContentType(testValue.value__)
        
        # assert value
        assert py.content_type == None if testValue is None else PyExtractRefreshContentType(testValue.value__)
    
    def test_content_getter(self):
        dotnet = self.create(IExtractRefreshTask[ISchedule])
        py = PyExtractRefreshTask[PySchedule](dotnet)
        assert py.content == None if dotnet.Content is None else PyContentReference(dotnet.Content)
    
    def test_content_setter(self):
        dotnet = self.create(IExtractRefreshTask[ISchedule])
        py = PyExtractRefreshTask[PySchedule](dotnet)
        
        # create test data
        testValue = self.create(IContentReference)
        
        # set property to new test value
        py.content = None if testValue is None else PyContentReference(testValue)
        
        # assert value
        assert py.content == None if testValue is None else PyContentReference(testValue)
    
class TestPyFrequencyDetailsGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IFrequencyDetails)
        py = PyFrequencyDetails(dotnet)
        assert py._dotnet == dotnet
    
    def test_start_at_getter(self):
        dotnet = self.create(IFrequencyDetails)
        py = PyFrequencyDetails(dotnet)
        assert py.start_at == None if dotnet.StartAt is None else time(dotnet.StartAt.Hour, dotnet.StartAt.Minute, dotnet.StartAt.Second, dotnet.StartAt.Millisecond * 1000)
    
    def test_start_at_setter(self):
        dotnet = self.create(IFrequencyDetails)
        py = PyFrequencyDetails(dotnet)
        
        # create test data
        testValue = self.create(Nullable[TimeOnly])
        
        # set property to new test value
        py.start_at = None if testValue is None else time(testValue.Hour, testValue.Minute, testValue.Second, testValue.Millisecond * 1000)
        
        # assert value
        assert py.start_at == None if testValue is None else time(testValue.Hour, testValue.Minute, testValue.Second, testValue.Millisecond * 1000)
    
    def test_end_at_getter(self):
        dotnet = self.create(IFrequencyDetails)
        py = PyFrequencyDetails(dotnet)
        assert py.end_at == None if dotnet.EndAt is None else time(dotnet.EndAt.Hour, dotnet.EndAt.Minute, dotnet.EndAt.Second, dotnet.EndAt.Millisecond * 1000)
    
    def test_end_at_setter(self):
        dotnet = self.create(IFrequencyDetails)
        py = PyFrequencyDetails(dotnet)
        
        # create test data
        testValue = self.create(Nullable[TimeOnly])
        
        # set property to new test value
        py.end_at = None if testValue is None else time(testValue.Hour, testValue.Minute, testValue.Second, testValue.Millisecond * 1000)
        
        # assert value
        assert py.end_at == None if testValue is None else time(testValue.Hour, testValue.Minute, testValue.Second, testValue.Millisecond * 1000)
    
    def test_intervals_getter(self):
        dotnet = self.create(IFrequencyDetails)
        py = PyFrequencyDetails(dotnet)
        assert len(dotnet.Intervals) != 0
        assert len(py.intervals) == len(dotnet.Intervals)
    
    def test_intervals_setter(self):
        dotnet = self.create(IFrequencyDetails)
        py = PyFrequencyDetails(dotnet)
        assert len(dotnet.Intervals) != 0
        assert len(py.intervals) == len(dotnet.Intervals)
        
        # create test data
        dotnetCollection = DotnetList[IInterval]()
        dotnetCollection.Add(self.create(IInterval))
        dotnetCollection.Add(self.create(IInterval))
        testCollection = [] if dotnetCollection is None else [PyInterval(x) for x in dotnetCollection if x is not None]
        
        # set property to new test value
        py.intervals = testCollection
        
        # assert value
        assert len(py.intervals) == len(testCollection)
    
class TestPyIntervalGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IInterval)
        py = PyInterval(dotnet)
        assert py._dotnet == dotnet
    
    def test_hours_getter(self):
        dotnet = self.create(IInterval)
        py = PyInterval(dotnet)
        assert py.hours == dotnet.Hours
    
    def test_minutes_getter(self):
        dotnet = self.create(IInterval)
        py = PyInterval(dotnet)
        assert py.minutes == dotnet.Minutes
    
    def test_month_day_getter(self):
        dotnet = self.create(IInterval)
        py = PyInterval(dotnet)
        assert py.month_day == dotnet.MonthDay
    
    def test_week_day_getter(self):
        dotnet = self.create(IInterval)
        py = PyInterval(dotnet)
        assert py.week_day == dotnet.WeekDay
    
class TestPyScheduleGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ISchedule)
        py = PySchedule(dotnet)
        assert py._dotnet == dotnet
    
    def test_frequency_getter(self):
        dotnet = self.create(ISchedule)
        py = PySchedule(dotnet)
        assert py.frequency == dotnet.Frequency
    
    def test_frequency_setter(self):
        dotnet = self.create(ISchedule)
        py = PySchedule(dotnet)
        
        # create test data
        testValue = self.create(String)
        
        # set property to new test value
        py.frequency = testValue
        
        # assert value
        assert py.frequency == testValue
    
    def test_frequency_details_getter(self):
        dotnet = self.create(ISchedule)
        py = PySchedule(dotnet)
        assert py.frequency_details == None if dotnet.FrequencyDetails is None else PyFrequencyDetails(dotnet.FrequencyDetails)
    
    def test_next_run_at_getter(self):
        dotnet = self.create(ISchedule)
        py = PySchedule(dotnet)
        assert py.next_run_at == dotnet.NextRunAt
    
class TestPyWithScheduleGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IWithSchedule[ISchedule])
        py = PyWithSchedule[PySchedule](dotnet)
        assert py._dotnet == dotnet
    
    def test_schedule_getter(self):
        dotnet = self.create(IWithSchedule[ISchedule])
        py = PyWithSchedule[PySchedule](dotnet)
        assert py.schedule == None if dotnet.Schedule is None else _generic_wrapper(dotnet.Schedule)
    

# endregion

