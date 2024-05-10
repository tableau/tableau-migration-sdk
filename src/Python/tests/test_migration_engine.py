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

# Make sure the test can find the module
import pytest

from tableau_migration.migration_engine import (
    PyServerToCloudMigrationPlanBuilder)

from Tableau.Migration import (
    IServerToCloudMigrationPlanBuilder)

import Moq

class TestPyServerToCloudMigrationPlanBuilder:
    def test_with_authentication_type_string_arg(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_authentication_type("auth", "userDomain", "groupDomain")

    def test_with_tableau_cloud_usernames_string_arg(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_tableau_cloud_usernames("test.com")
