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

from autofixture import AutoFixtureTestBase 

from default_project_filter import DefaultProjectFilter

from tableau_migration import ContentMigrationItem
from tableau_migration import IProject

from Tableau.Migration.Content import IProject as DotnetIProject
from Tableau.Migration.Engine import ContentMigrationItem as DotnetContentMigrationItem

class TestDefaultProjectFilter(AutoFixtureTestBase):
    def test_init(self):
        DefaultProjectFilter()
        
    def test_should_migrate(self):
        
        dotnet_item = self.create(DotnetContentMigrationItem[DotnetIProject])
        item = ContentMigrationItem[IProject](dotnet_item)
        
        filter = DefaultProjectFilter()        
        result = filter.should_migrate(item)
        
        assert item.source_item.name !='Default'
        assert result == True
        
        