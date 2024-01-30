# Copyright (c) 2023, Salesforce, Inc.
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

# This application is meant to use the migration modules directly from source.
# It builds the dotnet binaries and puts them on the path.

"""Python.TestApplication is meant to use the migration modules directly from source. It builds the dotnet binaries and puts them on the path."""

# Class python imports
import logging

import helper           # Helper code. Must be imported and called before TestComponents can be used 
import os
import sys
import time
import tableau_migration

from threading import Thread

# tableau_migration and migration_testcomponents
from tableau_migration.migration import PyMigrationResult, PyMigrationManifest
from tableau_migration.migration_engine import PyMigrationPlanBuilder
from tableau_migration.migration_engine_migrators import PyMigrator
from migration_testcomponents_engine_manifest import PyMigrationManifestSerializer
from migration_testcomponents_filters import PySpecialUserFilter, PyUnlicensedUserFilter
from migration_testcomponents_hooks import (
    PySaveManifestAfterBatch_User, 
    PySaveManifestAfterBatch_Group, 
    PySaveManifestAfterBatch_Project, 
    PySaveManifestAfterBatch_DataSource, 
    PySaveManifestAfterBatch_Workbook, 
    PyTimeLoggerAfterActionHook, 
    save_manifest_after_batch_user_factory, 
    save_manifest_after_batch_group_factory,
    save_manifest_after_batch_project_factory,
    save_manifest_after_batch_datasource_factory,
    save_manifest_after_batch_workbook_factory)
from migration_testcomponents_mappings import PySpecialUserMapping, PyTestTableauCloudUsernameMapping, PyUnlicensedUserMapping

# CSharp imports
from System import Func, IServiceProvider 

from Tableau.Migration.Content import IUser
from Tableau.Migration.Engine.Pipelines import ServerToCloudMigrationPipeline
from Tableau.Migration.Engine.Manifest import MigrationManifestEntryStatus
from Tableau.Migration.TestComponents import IServiceCollectionExtensions as MigrationTestComponentsSCE


class Program():
    """Main program class."""

    def __init__(self):
        """Program init, sets up logging."""
        # Send all log messages to file
        filename_template = helper.config['log']['folder_path'] + "Python.TestApplication-" + helper.now_str + ".log"
        logging.basicConfig(filename = filename_template, format='%(asctime)s|%(levelname)s|%(name)s|%(message)s', level=logging.INFO)

        # Create main migration app logger
        self.logger = logging.getLogger("Migration App")
        self.logger.setLevel(logging.DEBUG)

        # main migration app logger should write to console as well
        self.consoleHandler = logging.StreamHandler(sys.stdout)
        self.consoleHandler.setFormatter(logging.Formatter("[%(asctime)s %(levelname).3s] %(message)s", datefmt = "%H:%M:%S"))
        self.logger.addHandler(self.consoleHandler)

    def load_manifest(self, manifest_path: str) -> PyMigrationManifest:
        """Loads a manifest if requested."""
        manifest = self._manifest_serializer.load(manifest_path)
    
        if manifest is not None:
            while True:
                answer = input(f'Existing Manifest found at {manifest_path}. Should it be used? [Y/n] ').upper()

                if answer == 'N':
                    return None
                elif answer == 'Y' or answer == '':
                    return manifest
                
        return None

    def print_result(self, result: PyMigrationResult):
        """Prints the result of a migration."""
        self.logger.info(f'Result: {result.status}')
    
        for pipeline_content_type in ServerToCloudMigrationPipeline.ContentTypes:
            content_type = pipeline_content_type.ContentType
        
            type_result = result.manifest.entries.ForContentType(content_type)
        
            count_total = type_result.Count

            count_migrated = 0
            count_skipped = 0
            count_errored = 0
            count_cancelled = 0
            count_pending = 0

            for item in type_result:
                if item.Status == MigrationManifestEntryStatus.Migrated:
                    count_migrated += 1
                elif item.Status == MigrationManifestEntryStatus.Skipped:
                    count_skipped += 1
                elif item.Status == MigrationManifestEntryStatus.Error:
                    count_errored += 1
                elif item.Status == MigrationManifestEntryStatus.Canceled:
                    count_cancelled += 1
                elif item.Status == MigrationManifestEntryStatus.Pending:
                    count_pending += 1
            
            output = f'''
            {content_type.Name}
            \t{count_migrated}/{count_total} succeeded
            \t{count_skipped}/{count_total} skipped
            \t{count_errored}/{count_total} errored
            \t{count_cancelled}/{count_total} cancelled
            \t{count_pending}/{count_total} pending
            '''
               
            self.logger.info(output)

    def migrate(self):
        """The main migration function."""
        self.logger.info("Starting migration")
        # Add the C# test components we've ported to Python and register them with the DI Service Provider
        MigrationTestComponentsSCE.AddTestComponents(tableau_migration._service_collection)
        tableau_migration.migration._build_service_provider()

        # Setup base objects for migrations
        plan_builder = PyMigrationPlanBuilder()
        migration = PyMigrator()

        self._manifest_serializer = PyMigrationManifestSerializer()
    
        # Build the plan
        plan_builder = plan_builder \
                    .from_source_tableau_server(
                        server_url = helper.config['source']['url'], 
                        site_content_url = helper.config['source']['site_content_url'], 
                        access_token_name = helper.config['source']['access_token_name'], 
                        access_token = os.environ.get('TABLEAU_MIGRATION_SOURCE_TOKEN', helper.config['source']['access_token'])) \
                    .to_destination_tableau_cloud(
                        pod_url = helper.config['destination']['url'], 
                        site_content_url = helper.config['destination']['site_content_url'], 
                        access_token_name = helper.config['destination']['access_token_name'], 
                        access_token = os.environ.get('TABLEAU_MIGRATION_DESTINATION_TOKEN', helper.config['destination']['access_token'])) \
                    .for_server_to_cloud() \
                    .with_tableau_id_authentication_type() \
                    .with_tableau_cloud_usernames(PyTestTableauCloudUsernameMapping())        
    
    
        self.logger.info("Adding Special User filter and mapping")
        plan_builder.filters.add(IUser, PySpecialUserFilter())
        plan_builder.mappings.add(IUser, PySpecialUserMapping())
    
        self.logger.info("Adding unlicensed user filter and mapping")
        plan_builder.filters.add(IUser, PyUnlicensedUserFilter())
        plan_builder.mappings.add(IUser, PyUnlicensedUserMapping())
    
        self.logger.info("Adding Hooks")
        plan_builder.hooks.add(PyTimeLoggerAfterActionHook(self.consoleHandler))
    
        # This adds the PySaveManifestAfterBatch_<content type> hook via the save_manifest_after_batch_<content type>_factory function.
        # This is required because the manifest comes from the IMigration, which is scoped.
        # The global DI provider can not create the scoped IMigration.
        # the save_manifest_after_batch_<content type>_factory function takes the IServiceProvider, which is scoped when save_manifest_after_batch_<content type>_factory is called
        # This means save_manifest_after_batch_<content type>_factory can create the PySaveManifestAfterBatch_<content type> and pass in the IMigration
        plan_builder.hooks.add(PySaveManifestAfterBatch_User, Func[IServiceProvider, PySaveManifestAfterBatch_User](save_manifest_after_batch_user_factory))
        plan_builder.hooks.add(PySaveManifestAfterBatch_Group, Func[IServiceProvider, PySaveManifestAfterBatch_Group](save_manifest_after_batch_group_factory))
        plan_builder.hooks.add(PySaveManifestAfterBatch_Project, Func[IServiceProvider, PySaveManifestAfterBatch_Project](save_manifest_after_batch_project_factory))
        plan_builder.hooks.add(PySaveManifestAfterBatch_DataSource, Func[IServiceProvider, PySaveManifestAfterBatch_DataSource](save_manifest_after_batch_datasource_factory))
        plan_builder.hooks.add(PySaveManifestAfterBatch_Workbook, Func[IServiceProvider, PySaveManifestAfterBatch_Workbook](save_manifest_after_batch_workbook_factory))
    
        # Comment out an neccesary 
        #planBuilder.filters.add(IUser, PySkipFilter_Users())
        #planBuilder.filters.add(IGroup, PySkipFilter_Groups())
        #planBuilder.filters.add(IProject, PySkipFilter_Projects())
        #planBuilder.filters.add(IDataSource, PySkipFilter_DataSources())
        #planBuilder.filters.add(IWorkbook, PySkipFilter_Workbooks())

        # Load manifest if available
        prev_manifest = self.load_manifest(helper.config['previous_manifest'])

        start_time = time.time()

        # Run the migration. 
        plan = plan_builder.build()
        result = migration.execute(plan, prev_manifest)
         
        end_time = time.time()

        # Save the manifest.    
        self._manifest_serializer.save(result.manifest, helper.manifest_path)

        self.print_result(result)    

        self.logger.info(f'Migration Started: {time.ctime(start_time)}')
        self.logger.info(f'Migration Ended: {time.ctime(end_time)}')
        self.logger.info(f'Elapsed: {end_time - start_time}')

        print("All done")    
        

if __name__ == '__main__':
    
    program = Program()
    thread = Thread(target = program.migrate)
    thread.start()
    done = False

    while not done:
        try:
            thread.join(1)
        except KeyboardInterrupt:
            print("Caught Ctrl+C, shutting down...")
            tableau_migration.cancellation_token_source.Cancel()
            thread.join()
            done = True
            
