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

import os
import logging
from tableau_migration import _logger_names
from tableau_migration.migration import (
    get_service_provider,
    get_service
)

from tableau_migration.migration_engine import (
    PyMigrationPlanBuilder)

from Tableau.Migration.Config import IConfigReader
from Tableau.Migration.Content import IUser
from Tableau.Migration.Content import IWorkbook

class TestEndToEnd():
    def test_main(self):
        '''This is mean to mimic a real application'''
        planBuilder = PyMigrationPlanBuilder()

        planBuilder = planBuilder \
                .from_source_tableau_server("http://fakeSourceServer.com", "", "fakeTokenName", "fakeTokenValue") \
                .to_destination_tableau_cloud("https://online.tableau.com", "", "fakeTokenName", "fakeTokenValue") \
                .for_server_to_cloud() \
                .with_tableau_cloud_usernames("salesforce.com") \
                .with_tableau_id_authentication_type()  

        result = planBuilder.validate();
        assert result.success is True

class TestLogging():
    def test_logging(self):
        '''At this point the module __init()__ has already been called and the logging provider factory has been initialized'''

        # Create a migration sdk object so that an ILogger will be instaniated
        PyMigrationPlanBuilder()

        # tableau_migration module keeps track of instaniated loggers. Verify that we have at least one
        assert len(_logger_names) > 0

        for name in _logger_names:
            # Given that we have a name, we should have a logger
            assert logging.getLogger(name)            

class TestConfig():
    def test_config(self):
        '''
        Verify that the MigrationSDK__BatchSize variable has been changed
        Verify that the MigrationSDK__IncludeExtractEnabled variable has been changed
        
        Environment variables act different in different operating systems. For this reason we 
        must set the MigrationSDK_BatchSize in pytest.ini, as that is set before the python
        process starts and hence the env variables take and that way an int can be set to a env var.
        '''

        services = get_service_provider()
        config_reader = get_service(services, IConfigReader)

        batch_size = config_reader.Get[IUser]().BatchSize
        include_extract = config_reader.Get[IWorkbook]().IncludeExtractEnabled

        assert batch_size == 102
        assert include_extract is False
