from tableau_migration.migration import PyMigrationResult
from tableau_migration import IMigrationManifestEntry, MigrationManifestEntryStatus
from Tableau.Migration.Engine.Pipelines import ServerToCloudMigrationPipeline

def print_result(result: PyMigrationResult):
        """Prints the result of a migration."""
        print(f'Result: {result.status}')
    
        for pipeline_content_type in ServerToCloudMigrationPipeline.ContentTypes:
            content_type = pipeline_content_type.ContentType
            
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
               
            print(output)