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

from tableau_migration.migration import PyContentReference # noqa: E402, F401
from tableau_migration.migration_content_schedules import PySchedule # noqa: E402, F401

from Tableau.Migration.Content.Schedules.Server import IServerSchedule # noqa: E402, F401

from tableau_migration.migration_content_schedules_server import PyServerSchedule # noqa: E402, F401
# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyServerScheduleGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IServerSchedule)
        py = PyServerSchedule(dotnet)
        assert py._dotnet == dotnet
    
    def test_type_getter(self):
        dotnet = self.create(IServerSchedule)
        py = PyServerSchedule(dotnet)
        assert py.type == dotnet.Type
    
    def test_state_getter(self):
        dotnet = self.create(IServerSchedule)
        py = PyServerSchedule(dotnet)
        assert py.state == dotnet.State
    
    def test_created_at_getter(self):
        dotnet = self.create(IServerSchedule)
        py = PyServerSchedule(dotnet)
        assert py.created_at == dotnet.CreatedAt
    
    def test_updated_at_getter(self):
        dotnet = self.create(IServerSchedule)
        py = PyServerSchedule(dotnet)
        assert py.updated_at == dotnet.UpdatedAt
    

# endregion

from Tableau.Migration.Content.Schedules.Server import IServerExtractRefreshTask # noqa: E402, F401

from tableau_migration.migration_content_schedules_server import PyServerExtractRefreshTask # noqa: E402, F401

class TestPyServerExtractRefreshTask(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IServerExtractRefreshTask)
        py = PyServerExtractRefreshTask(dotnet)
        assert py._dotnet == dotnet        

