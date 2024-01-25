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