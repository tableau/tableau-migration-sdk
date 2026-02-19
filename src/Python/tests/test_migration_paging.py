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

import pytest

from tableau_migration import cancellation_token
from tableau_migration.migration_content import PyUser
from tableau_migration.migration_paging import empty_pager, memory_pager

from Tableau.Migration.Content import IUser
from Tableau.Migration.Paging import IPager

from tests.helpers.autofixture import AutoFixtureTestBase

class TestEmptyPager(AutoFixtureTestBase):
    def test_creates_empty_pager(self):
        pager = empty_pager(PyUser)

        assert pager is not None
        assert isinstance(pager, IPager[IUser])

        page = pager.NextPageAsync(cancellation_token).GetAwaiter().GetResult()

        assert page is not None

        assert page.PageSize == 1
        assert len(page.Value) == 0

class TestMemoryPager(AutoFixtureTestBase):
    def test_creates_memory_pager(self):
        items = [PyUser(self.create(IUser)), PyUser(self.create(IUser)), PyUser(self.create(IUser))]
        
        pager = memory_pager(PyUser, items, 50)

        assert pager is not None
        assert isinstance(pager, IPager[IUser])

        page = pager.NextPageAsync(cancellation_token).GetAwaiter().GetResult()

        assert page is not None

        assert page.PageSize == 50
        assert len(page.Value) == len(items)