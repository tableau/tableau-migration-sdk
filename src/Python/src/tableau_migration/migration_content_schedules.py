# Copyright (c) 2025, Salesforce, Inc.
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

"""Wrapper for classes in Tableau.Migration.Content.Schedules namespace."""

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

TSchedule = TypeVar("TSchedule")

class PyExtractRefreshContentType(IntEnum):
    """Enum of extract refresh content types."""
    
    """Unknown content type."""
    UNKNOWN = 0
    
    """Workbook content type."""
    WORKBOOK = 1
    
    """Data source content type."""
    DATA_SOURCE = 2
    
class PyWithSchedule(Generic[TSchedule], PyContentReference):
    """Interface to be inherited by content items with a schedule."""
    
    _dotnet_base = IWithSchedule
    
    def __init__(self, with_schedule: IWithSchedule) -> None:
        """Creates a new PyWithSchedule object.
        
        Args:
            with_schedule: A IWithSchedule object.
        
        Returns: None.
        """
        self._dotnet = with_schedule
        
    @property
    def schedule(self) -> TSchedule:
        """Gets the content item's schedule."""
        return None if self._dotnet.Schedule is None else _generic_wrapper(self._dotnet.Schedule)
    
class PyExtractRefreshTask(Generic[TSchedule], PyWithSchedule[TSchedule]):
    """Interface for an extract refresh task content item."""
    
    _dotnet_base = IExtractRefreshTask
    
    def __init__(self, extract_refresh_task: IExtractRefreshTask) -> None:
        """Creates a new PyExtractRefreshTask object.
        
        Args:
            extract_refresh_task: A IExtractRefreshTask object.
        
        Returns: None.
        """
        self._dotnet = extract_refresh_task
        
    @property
    def type(self) -> str:
        """Gets the extract refresh type."""
        return self._dotnet.Type
    
    @type.setter
    def type(self, value: str) -> None:
        """Gets the extract refresh type."""
        self._dotnet.Type = value
    
    @property
    def content_type(self) -> PyExtractRefreshContentType:
        """Gets the extract refresh task's content type."""
        return None if self._dotnet.ContentType is None else PyExtractRefreshContentType(self._dotnet.ContentType.value__)
    
    @content_type.setter
    def content_type(self, value: PyExtractRefreshContentType) -> None:
        """Gets the extract refresh task's content type."""
        self._dotnet.ContentType.value__ = PyExtractRefreshContentType(value)
    
    @property
    def content(self) -> PyContentReference:
        """Gets the extract refresh task's content."""
        return None if self._dotnet.Content is None else PyContentReference(self._dotnet.Content)
    
    @content.setter
    def content(self, value: PyContentReference) -> None:
        """Gets the extract refresh task's content."""
        self._dotnet.Content = None if value is None else value._dotnet
    
class PyInterval():
    """Interface for a schedule interval."""
    
    _dotnet_base = IInterval
    
    def __init__(self, interval: IInterval) -> None:
        """Creates a new PyInterval object.
        
        Args:
            interval: A IInterval object.
        
        Returns: None.
        """
        self._dotnet = interval
        
    @property
    def hours(self) -> int:
        """Gets the interval hour value."""
        return self._dotnet.Hours
    
    @property
    def minutes(self) -> int:
        """Gets the interval minute value."""
        return self._dotnet.Minutes
    
    @property
    def month_day(self) -> str:
        """Gets the interval day of month value."""
        return self._dotnet.MonthDay
    
    @property
    def week_day(self) -> str:
        """Gets the interval day of week value."""
        return self._dotnet.WeekDay
    
class PyFrequencyDetails():
    """Interface for a schedule's frequency details."""
    
    _dotnet_base = IFrequencyDetails
    
    def __init__(self, frequency_details: IFrequencyDetails) -> None:
        """Creates a new PyFrequencyDetails object.
        
        Args:
            frequency_details: A IFrequencyDetails object.
        
        Returns: None.
        """
        self._dotnet = frequency_details
        
    @property
    def start_at(self) -> time:
        """Gets the schedule's start time."""
        return None if self._dotnet.StartAt is None else time(self._dotnet.StartAt.Hour, self._dotnet.StartAt.Minute, self._dotnet.StartAt.Second, self._dotnet.StartAt.Millisecond * 1000)
    
    @start_at.setter
    def start_at(self, value: time) -> None:
        """Gets the schedule's start time."""
        self._dotnet.StartAt = None if value is None else TimeOnly.Parse(str(value))
    
    @property
    def end_at(self) -> time:
        """Gets the schedule's end time."""
        return None if self._dotnet.EndAt is None else time(self._dotnet.EndAt.Hour, self._dotnet.EndAt.Minute, self._dotnet.EndAt.Second, self._dotnet.EndAt.Millisecond * 1000)
    
    @end_at.setter
    def end_at(self, value: time) -> None:
        """Gets the schedule's end time."""
        self._dotnet.EndAt = None if value is None else TimeOnly.Parse(str(value))
    
    @property
    def intervals(self) -> List[PyInterval]:
        """Gets the schedule's intervals."""
        return [] if self._dotnet.Intervals is None else [PyInterval(x) for x in self._dotnet.Intervals if x is not None]
    
    @intervals.setter
    def intervals(self, value: List[PyInterval]) -> None:
        """Gets the schedule's intervals."""
        if value is None:
            self._dotnet.Intervals = DotnetList[IInterval]()
        else:
            dotnet_collection = DotnetList[IInterval]()
            for x in filter(None,value):
                dotnet_collection.Add(x._dotnet)
            self._dotnet.Intervals = dotnet_collection
    
class PySchedule():
    """Interface for an API client schedule model."""
    
    _dotnet_base = ISchedule
    
    def __init__(self, schedule: ISchedule) -> None:
        """Creates a new PySchedule object.
        
        Args:
            schedule: A ISchedule object.
        
        Returns: None.
        """
        self._dotnet = schedule
        
    @property
    def frequency(self) -> str:
        """Gets the schedule's frequency."""
        return self._dotnet.Frequency
    
    @frequency.setter
    def frequency(self, value: str) -> None:
        """Gets the schedule's frequency."""
        self._dotnet.Frequency = value
    
    @property
    def frequency_details(self) -> PyFrequencyDetails:
        """Gets the schedule's frequency details."""
        return None if self._dotnet.FrequencyDetails is None else PyFrequencyDetails(self._dotnet.FrequencyDetails)
    
    @property
    def next_run_at(self) -> str:
        """Gets the schedule's next run time."""
        return self._dotnet.NextRunAt
    

# endregion

