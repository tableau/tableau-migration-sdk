# This application is meant to use the migration modules directly from source.
# It builds the dotnet binaries and puts them on the path.

# configuration parser  
import configparser                                                                                         # noqa: F401
# Helper code. Must be imported and called before TestComponents can be used 
import helper
# environment variables
import os
import tableau_migration

# Imports from tableau_migration module.
from tableau_migration.migration_engine import PyMigrationPlanBuilder                                       # noqa: F401, E402
from tableau_migration.migration import PyMigrationManifest                                                 # noqa: F401, E402
from tableau_migration.migration_engine_migrators import PyMigrator                                         # noqa: F401, E402
from tableau_migration.migration_engine import PyServerToCloudMigrationPlanBuilder                          # noqa: F401, E402
from migration_testcomponents_engine_manifest import PyMigrationManifestSerializer                          # noqa: F401, E402

# Imports from TestComponents
from Tableau.Migration.TestComponents import IServiceCollectionExtensions as MigrationTestComponentsSCE     # noqa: F401, E402
from Microsoft.Extensions.DependencyInjection import ServiceCollectionContainerBuilderExtensions            # noqa: F401, E402
 

from Tableau.Migration.TestComponents.Hooks.Mappings import TestTableauCloudUsernameMapping                 # noqa: F401, E402
from System.Net.Mail import MailAddress                                                                     # noqa: F401, E402

if __name__ == '__main__':
    MigrationTestComponentsSCE.AddTestComponents(tableau_migration._service_collection)
    tableau_migration._services = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(tableau_migration._service_collection)
    planBuilder = PyMigrationPlanBuilder()
    migration = PyMigrator()
    manifestSerializer = PyMigrationManifestSerializer()

    config = configparser.ConfigParser()
    config.read('config.ini')
    config.read('config.DEV.ini')

    mail_address = MailAddress(config['MIGRATION_OPTIONS']['BASE_OVERRIDE_MAIL_ADDRESS'])
    mapping = TestTableauCloudUsernameMapping(mail_address)

    # Build the plan
    planBuilder = planBuilder \
                .from_source_tableau_server(config['SOURCE']['URL'], config['SOURCE']['SITE_CONTENT_URL'], config['SOURCE']['ACCESS_TOKEN_NAME'], os.environ['TABLEAU_MIGRATION_SOURCE_TOKEN']) \
                .to_destination_tableau_cloud(config['DESTINATION']['URL'], config['DESTINATION']['SITE_CONTENT_URL'], config['DESTINATION']['ACCESS_TOKEN_NAME'], os.environ['TABLEAU_MIGRATION_DESTINATION_TOKEN']) \
                .for_server_to_cloud() \
                .with_tableau_id_authentication_type() \
                .with_tableau_cloud_usernames(mapping)
    
    plan = planBuilder.build()

    # Load the manifest file if one exists.
    manifest=manifestSerializer.load(helper._manifestPath)
    
    # Run the migration. 
    result = migration.execute(plan,manifest)
         
    # Save the manifest.    
    manifestSerializer.save(result.manifest,helper._manifestPath)

    print("All done")