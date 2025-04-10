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

# Make sure the test can find the module
import pytest

from tableau_migration import (
    ContentMappingContext as PyContentMappingContext,
    IUser as PyUser,
    MigrationPlanBuilder,
    TableauCloudUsernameMappingBase)

from tableau_migration.migration_engine import PyServerToCloudMigrationPlanBuilder

from System import IServiceProvider
from System.Threading import CancellationToken
from Tableau.Migration import IServerToCloudMigrationPlanBuilder
from Tableau.Migration.Content import IUser
from Tableau.Migration.Engine.Hooks import IMigrationHook
from Tableau.Migration.Engine.Hooks.Mappings import (
    ContentMappingContext,
    IContentMapping)

from tests.helpers.autofixture import AutoFixtureTestBase
import Moq

class TestUsernameMapping(TableauCloudUsernameMappingBase):
    """Mapping that takes a base email and appends the source item name to the email username."""

    def map(self, ctx: PyContentMappingContext[PyUser]) -> PyContentMappingContext[PyUser]:  # noqa: N802
        return ctx

class TestPyServerToCloudMigrationPlanBuilder(AutoFixtureTestBase):
    def test_with_saml_authentication_type_auth_type(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_saml_authentication_type("userDomain")

    def test_with_saml_authentication_type_idp_name(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_saml_authentication_type("userDomain", "idp name")

    def test_with_tableau_id_authentication_type_auth_type(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_tableau_id_authentication_type()

    def test_with_tableau_id_authentication_type_idp_name(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_tableau_id_authentication_type(True, "idp name")
    
    def test_with_authentication_type_string_arg_auth_type(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_authentication_type("auth", "userDomain", "groupDomain")

    def test_with_tableau_cloud_usernames_string_arg(self):
        mockBuilder = Moq.Mock[IServerToCloudMigrationPlanBuilder]()
        builder = PyServerToCloudMigrationPlanBuilder(mockBuilder.Object)
        
        builder.with_tableau_cloud_usernames("test.com")

    def test_with_tableau_cloud_usernames_class(self):
        builder = MigrationPlanBuilder()
        
        builder = builder.for_server_to_cloud().with_tableau_cloud_usernames(TestUsernameMapping)

        hook_builder = builder.mappings.build()
        hook_factories = hook_builder.get_hooks(IContentMapping[IUser])

        services = self.create(IServiceProvider)
        hook = hook_factories[0].Create[IMigrationHook[ContentMappingContext[IUser]]](services)

        map_ctx = self.create(ContentMappingContext[IUser])
        hook_result = hook.ExecuteAsync(map_ctx, CancellationToken(False)).GetAwaiter().GetResult()

