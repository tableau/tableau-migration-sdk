# This application performs a basic migration using the Tableau Migration SDK.
# By default all supported content will be migrated, but can be modified to your specific needs.
# The application assumes you have already installed the Tableau Migration SDK Python package.

from dotenv import load_dotenv

load_dotenv()

import configparser          # configuration parser
import os                    # environment variables
import tableau_migration     # Tableau Migration SDK
from print_result import print_result

from threading import Thread # threading

from tableau_migration import (
    MigrationManifestSerializer,
    MigrationManifest
)

serializer = MigrationManifestSerializer()

def migrate():
    """Performs a migration using Tableau Migration SDK."""
    
    # Get the absolute path of the current file
    current_file_path = os.path.abspath(__file__)
    manifest_path = os.path.join(os.path.dirname(current_file_path), 'manifest.json')

    plan_builder = tableau_migration.MigrationPlanBuilder()
    migration = tableau_migration.Migrator()

    config = configparser.ConfigParser()
    config.read('config.ini')

    # Build the plan.
    plan_builder = plan_builder \
                    .from_source_tableau_server(
                        server_url = config['SOURCE']['URL'], 
                        site_content_url = config['SOURCE']['SITE_CONTENT_URL'], 
                        access_token_name = config['SOURCE']['ACCESS_TOKEN_NAME'], 
                        access_token = os.environ.get('TABLEAU_MIGRATION_SOURCE_TOKEN', config['SOURCE']['ACCESS_TOKEN']),
                        create_api_simulator = os.environ.get('TABLEAU_MIGRATION_SOURCE_SIMULATION', 'False') == 'True') \
                    .to_destination_tableau_cloud(
                        pod_url = config['DESTINATION']['URL'], 
                        site_content_url = config['DESTINATION']['SITE_CONTENT_URL'], 
                        access_token_name = config['DESTINATION']['ACCESS_TOKEN_NAME'], 
                        access_token = os.environ.get('TABLEAU_MIGRATION_DESTINATION_TOKEN', config['DESTINATION']['ACCESS_TOKEN']),
                        create_api_simulator = os.environ.get('TABLEAU_MIGRATION_DESTINATION_SIMULATION', 'False') == 'True') \
                    .for_server_to_cloud() \
                    .with_tableau_id_authentication_type() \
                    .with_tableau_cloud_usernames(config['USERS']['EMAIL_DOMAIN'])                    

    # TODO: add filters, mappings, transformers, etc. here.


    # Load the previous manifest file if it exists.
    prev_manifest = load_manifest(f'{manifest_path}')

    # Validate the migration plan.
    validation_result = plan_builder.validate()

    # TODO: Handle errors if the validation fails here.

    plan = plan_builder.build()

    # Run the migration.
    results = migration.execute(plan, prev_manifest)
    
    # Save the manifest file.
    serializer.save(results.manifest, f'{manifest_path}')

    # TODO: Handle results here.
    print_result(results)

    print("All done.")

def load_manifest(manifest_path: str) -> MigrationManifest | None:
        """Loads a manifest if requested."""
        manifest = serializer.load(manifest_path)
    
        if manifest is not None:
            while True:
                answer = input(f'Existing Manifest found at {manifest_path}. Should it be used? [Y/n] ').upper()

                if answer == 'N':
                    return None
                elif answer == 'Y' or answer == '':
                    return manifest
                
        return None
    

if __name__ == '__main__':
    
    # Create a thread that will run the migration and start it.
    migration_thread = Thread(target = migrate)
    migration_thread.start()
    done = False

    # Create a busy-wait loop to continue checking if Ctrl+C was pressed to cancel the migration.
    while not done:
        try:
            migration_thread.join(1)
            done = True
        except KeyboardInterrupt:
            # Ctrl+C was caught, request migration to cancel. 
            print("Caught Ctrl+C, shutting down...")
            
            # This will cause the Migration SDK to cleanup and finish,
            # which will cause the thread to finish.
            tableau_migration.cancellation_token_source.Cancel()
            
            # Wait for the migration thread to finish and then quit the application.
            migration_thread.join()
            done = True
