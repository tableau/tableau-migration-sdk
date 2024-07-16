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

# This application is meant to use the migration modules directly from source.
# It builds the dotnet binaries and puts them on the path.

"""Python.TestApplication is meant to use the migration modules directly from source. It builds the dotnet binaries and puts them on the path."""

# Class python imports
import logging

import helper
import os
import sys
import time
import tableau_migration

from threading import Thread

# tableau_migration and migration_testcomponents
from tableau_migration import (
    IMigrationManifestEntry, 
    Migrator, 
    MigrationPlanBuilder, 
    MigrationManifestEntryStatus,
    MigrationManifestSerializer,
    cancellation_token_source
)
from tableau_migration.migration import (
    PyMigrationResult, 
    PyMigrationManifest
)

from migration_testcomponents_filters import (
    # Skip Filters: Uncomment when neccesary.
    # SkipAllUsersFilter,
    # SkipAllGroupsFilter,
    # SkipAllProjectsFilter,
    # SkipAllDataSourcesFilter,
    # SkipAllWorkbooksFilter,
    # SkipAllExtractRefreshTasksFilter,
    # Skip Filters: Uncomment when neccesary.
    SkipProjectByParentLocationFilter,
    SkipDataSourceByParentLocationFilter,
    SkipWorkbookByParentLocationFilter,
    SpecialUserFilter, 
    UnlicensedUserFilter
)
from migration_testcomponents_hooks import (
    SaveUserManifestHook,
    SaveGroupManifestHook,
    SaveProjectManifestHook,
    SaveDataSourceManifestHook,
    SaveWorkbookManifestHook,
    TimeLoggerAfterActionHook
)
from migration_testcomponents_mappings import (
    SpecialUserMapping, 
    TestTableauCloudUsernameMapping,
    ProjectWithinSkippedLocationMapping,
    DataSourceWithinSkippedLocationMapping,
    WorkbookWithinSkippedLocationMapping,
    UnlicensedUserMapping
)
from migration_testcomponents_transformers import (
    RemoveMissingDestinationUsersFromGroupsTransformer
)

# CSharp imports
from Tableau.Migration.Engine.Pipelines import ServerToCloudMigrationPipeline


class Program():
    """Main program class."""
    
    done = False

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
        
            result.manifest.entries
            type_entries = [IMigrationManifestEntry(x) for x in result.manifest.entries.ForContentType(content_type)]
        
            count_total = len(type_entries)

            count_migrated = 0
            count_skipped = 0
            count_errored = 0
            count_cancelled = 0
            count_pending = 0

            for entry in type_entries:
                if entry.status == MigrationManifestEntryStatus.MIGRATED:
                    count_migrated += 1
                elif entry.status == MigrationManifestEntryStatus.SKIPPED:
                    count_skipped += 1
                elif entry.status == MigrationManifestEntryStatus.ERROR:
                    count_errored += 1
                elif entry.status == MigrationManifestEntryStatus.CANCELED:
                    count_cancelled += 1
                elif entry.status == MigrationManifestEntryStatus.PENDING:
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
        

        # Setup base objects for migrations
        self._manifest_serializer = MigrationManifestSerializer()
        plan_builder = MigrationPlanBuilder()
        migration = Migrator()
    
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
                    .with_tableau_cloud_usernames(TestTableauCloudUsernameMapping)        
    
    
        self.logger.info("Adding Special User filter and mapping")
        plan_builder.filters.add(SpecialUserFilter)
        plan_builder.mappings.add(SpecialUserMapping)
    
        self.logger.info("Adding unlicensed user filter and mapping")
        plan_builder.filters.add(UnlicensedUserFilter)
        plan_builder.mappings.add(UnlicensedUserMapping)
    
        self.logger.info("Adding Hooks")
        TimeLoggerAfterActionHook.handler = self.consoleHandler
        plan_builder.hooks.add(TimeLoggerAfterActionHook)
    
        plan_builder.hooks.add(SaveUserManifestHook)
        plan_builder.hooks.add(SaveGroupManifestHook)
        plan_builder.hooks.add(SaveProjectManifestHook)
        plan_builder.hooks.add(SaveDataSourceManifestHook)
        plan_builder.hooks.add(SaveWorkbookManifestHook)

        plan_builder.filters.add(SkipProjectByParentLocationFilter)
        plan_builder.filters.add(SkipDataSourceByParentLocationFilter)
        plan_builder.filters.add(SkipWorkbookByParentLocationFilter)
        plan_builder.transformers.add(RemoveMissingDestinationUsersFromGroupsTransformer)
        plan_builder.mappings.add(ProjectWithinSkippedLocationMapping)
        plan_builder.mappings.add(DataSourceWithinSkippedLocationMapping)
        plan_builder.mappings.add(WorkbookWithinSkippedLocationMapping)
    
        # Skip Filters: Uncomment when neccesary.
        # plan_builder.filters.add(SkipAllUsersFilter)
        # plan_builder.filters.add(SkipAllGroupsFilter)
        # plan_builder.filters.add(SkipAllProjectsFilter)
        # plan_builder.filters.add(SkipAllDataSourcesFilter)
        # plan_builder.filters.add(SkipAllWorkbooksFilter)
        # plan_builder.filters.add(SkipAllExtractRefreshTasksFilter)
        # Skip Filters: Uncomment when neccesary.

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

        self.done = True
        

if __name__ == '__main__':
    
    program = Program()
    thread = Thread(target = program.migrate)
    thread.start()

    while not program.done:
        try:
            thread.join(1)
        except KeyboardInterrupt:
            print("Caught Ctrl+C, shutting down...")
            cancellation_token_source.Cancel()
            thread.join()
            program.done = True
            
