
import logging
import tableau_migration

from tableau_migration.migration_engine import (
    PyMigrationPlanBuilder)

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
        assert len(tableau_migration._logger_names) > 0
        
        for name in tableau_migration._logger_names:
            # Given that we have a name, we should have a logger
            assert logging.getLogger(name)            
