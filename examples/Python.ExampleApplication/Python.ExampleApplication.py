# This application performs a basic migration using the Tableau Migration SDK.
# By default all supported content will be migrated, but can be modified to your specific needs.
# The application assumes you have already installed the Tableau Migration SDK Python package.

import configparser          # configuration parser
import os                    # environment variables
import sys                   # system utility
import tableau_migration     # Tableau Migration SDK

from threading import Thread # threading

def migrate():
    """Performs a migration using Tableau Migration SDK."""
    
    planBuilder = tableau_migration.MigrationPlanBuilder()
    migration = tableau_migration.Migrator()

    config = configparser.ConfigParser()
    config.read('config.ini')

    # Build the plan.
    plan_builder = plan_builder \
                    .from_source_tableau_server(
                        server_url = config['SOURCE']['URL'], 
                        site_content_url = config['SOURCE']['SITE_CONTENT_URL'], 
                        access_token_name = config['SOURCE']['ACCESS_TOKEN_NAME'], 
                        access_token = os.environ.get('TABLEAU_MIGRATION_SOURCE_TOKEN', config['SOURCE']['ACCESS_TOKEN'])) \
                    .to_destination_tableau_cloud(
                        pod_url = config['DESTINATION']['URL'], 
                        site_content_url = config['DESTINATION']['SITE_CONTENT_URL'], 
                        access_token_name = config['DESTINATION']['ACCESS_TOKEN_NAME'], 
                        access_token = os.environ.get('TABLEAU_MIGRATION_DESTINATION_TOKEN', config['DESTINATION']['ACCESS_TOKEN'])) \
                    .for_server_to_cloud() \
                    .with_tableau_id_authentication_type() \
                    .with_tableau_cloud_usernames(config['USERS']['EMAIL_DOMAIN'])

    # TODO: add filters, mappings, transformers, etc. here.

    # Validate the migration plan.
    validation_result = planBuilder.validate()

    # TODO: Handle errors if the validation fails here.

    plan = planBuilder.build()

    # Run the migration.
    results = migration.execute(plan)
    
    # TODO: Handle results here.

    print("All done.")


if __name__ == '__main__':
    
    # Create a thread that will run the migration and start it.
    migration_thread = Thread(target = migrate)
    migration_thread.start();
    done = False

    # Create a busy-wait loop to continue checking if Ctrl+C was pressed to cancel the migration.
    while not done:
        try:
            migration_thread.join(1)
            sys.exit(0)
        except KeyboardInterrupt:
            # Ctrl+C was caught, request migration to cancel. 
            print("Caught Ctrl+C, shutting down...")
            
            # This will cause the Migration SDK to cleanup and finish,
            # which will cause the thread to finish.
            tableau_migration.cancellation_token_source.Cancel()
            
            # Wait for the migration thread to finish and then quit the application.
            migration_thread.join()
            done = True
