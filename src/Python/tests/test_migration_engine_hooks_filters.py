# Copyright (c) 2026, Salesforce, Inc.
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

from tableau_migration.migration import PyContentReference # noqa: E402, F401

from Tableau.Migration import IContentReference # noqa: E402, F401

# region _generated

from enum import IntEnum # noqa: E402, F401
from tableau_migration.migration import _generic_wrapper # noqa: E402, F401
from tableau_migration.migration_engine import PyContentMigrationItem # noqa: E402, F401
from typing import (  # noqa: E402, F401
    Generic,
    TypeVar,
    Sequence
)

from Tableau.Migration.Engine.Hooks.Filters import (  # noqa: E402, F401
    ContentFilterContext,
    ContentFilterContextItem,
    FilterStatus
)

from tableau_migration.migration_engine_hooks_filters import (  # noqa: E402, F401
    PyContentFilterContext,
    PyContentFilterContextItem,
    PyFilterStatus
)


from Tableau.Migration.Engine.Hooks.Filters import FilterStatus

# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyContentFilterContextGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ContentFilterContext[IContentReference])
        py = PyContentFilterContext[PyContentReference](dotnet)
        assert py._dotnet == dotnet
    
    def test_items_getter(self):
        dotnet = self.create(ContentFilterContext[IContentReference])
        py = PyContentFilterContext[PyContentReference](dotnet)
        assert dotnet.Items.Count != 0
        assert len(py.items) == dotnet.Items.Count
    
class TestPyContentFilterContextItemGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(ContentFilterContextItem[IContentReference])
        py = PyContentFilterContextItem[PyContentReference](dotnet)
        assert py._dotnet == dotnet
    
    def test_status_getter(self):
        dotnet = self.create(ContentFilterContextItem[IContentReference])
        py = PyContentFilterContextItem[PyContentReference](dotnet)
        assert py.status.value == (None if dotnet.Status is None else PyFilterStatus(dotnet.Status.value__)).value
    
    def test_status_setter(self):
        dotnet = self.create(ContentFilterContextItem[IContentReference])
        py = PyContentFilterContextItem[PyContentReference](dotnet)
        
        # create test data
        testValue = self.create(FilterStatus)
        
        # set property to new test value
        py.status = None if testValue is None else PyFilterStatus(testValue.value__)
        
        # assert value
        assert py.status == None if testValue is None else PyFilterStatus(testValue.value__)
    

# endregion

