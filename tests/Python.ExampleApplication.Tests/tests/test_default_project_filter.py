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

from unittest.mock import MagicMock, patch

from autofixture import PySimpleAutoFixtureTestBase

from default_project_filter import DefaultProjectFilter

from tableau_migration import ContentFilterContextItem, FilterStatus, IProject
from tableau_migration.migration_engine_hooks_filters import PyContentFilterContextItem

from Tableau.Migration.Content import IProject as DotnetIProject
from Tableau.Migration.Engine.Hooks.Filters import ContentFilterContextItem as DotnetContentFilterContextItem


class TestDefaultProjectFilter(PySimpleAutoFixtureTestBase):
    def test_init(self):
        DefaultProjectFilter()

    def test_filter_non_default_project_migrates(self):
        dotnet_item = self.create(DotnetContentFilterContextItem[DotnetIProject])
        item = ContentFilterContextItem[IProject](dotnet_item)

        assert item.source_item.name.casefold() != 'default'

        DefaultProjectFilter().filter(item)

        assert item.status == FilterStatus.MIGRATE

    def test_filter_default_project_cascade_skips(self):
        dotnet_item = self.create(DotnetContentFilterContextItem[DotnetIProject])
        item = ContentFilterContextItem[IProject](dotnet_item)

        mock_project = MagicMock()
        mock_project.name = 'Default'

        with patch.object(PyContentFilterContextItem, 'source_item', new=property(lambda self: mock_project)):
            DefaultProjectFilter().filter(item)

        assert item.status == FilterStatus.CASCADE_SKIP
