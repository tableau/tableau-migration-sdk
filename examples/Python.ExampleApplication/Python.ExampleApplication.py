# This application is meant to mimic what an actual user would write

#region script

# This application assumes you have already installed the Tableau Migration SDK Python package.

import configparser         # configuration parser
import os                   # environment variables

from tableau_migration.migration_engine import PyMigrationPlanBuilder
from tableau_migration.migration_engine_migrators import PyMigrator

if __name__ == '__main__':
    planBuilder = PyMigrationPlanBuilder()
    migration = PyMigrator()

    config = configparser.ConfigParser()
    config.read('config.DEV.ini')

    # Build the plan
    planBuilder = planBuilder \
                .from_source_tableau_server(config['SOURCE']['URL'], config['SOURCE']['SITE_CONTENT_URL'], config['SOURCE']['ACCESS_TOKEN_NAME'], os.environ['TABLEAU_MIGRATION_SOURCE_TOKEN']) \
                .to_destination_tableau_cloud(config['DESTINATION']['URL'], config['DESTINATION']['SITE_CONTENT_URL'], config['DESTINATION']['ACCESS_TOKEN_NAME'], os.environ['TABLEAU_MIGRATION_DESTINATION_TOKEN']) \
                .for_server_to_cloud() \
                    .with_tableau_id_authentication_type() \
                    .with_tableau_cloud_usernames(config['USERS']['EMAIL_DOMAIN'])

    # You can Add filters, mappings, transformers, etc. here

    # Validate the migration plan
    validation_result=planBuilder.validate()

    #You can log errors here if the validation fails

    plan = planBuilder.build()

    # Run the migration
    results = migration.execute(plan)
    
    # You can handle results here

    print("All done")

#endregion    