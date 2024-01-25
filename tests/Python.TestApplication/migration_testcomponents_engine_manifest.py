"""Wrapper for classes in Tableau.Migration.TestComponents.Engine.Manifest namespace."""
import tableau_migration

from Tableau.Migration.TestComponents.Engine.Manifest import MigrationManifestSerializer
from System.Text.Json import JsonSerializerOptions
from tableau_migration.migration import PyMigrationManifest

class PyMigrationManifestSerializer():
    """A class to serialize the migration manifest."""
    _dotnet_base = MigrationManifestSerializer
    
    def __init__(self) -> None:
        """Default __init__."""
        self._services = tableau_migration.migration.get_service_provider()
        self._migration_manifest_serializer = tableau_migration.migration.get_service(self._services, MigrationManifestSerializer)
        
    def save(self, manifest: PyMigrationManifest, path: str, cancel = tableau_migration.cancellation_token, json_options: JsonSerializerOptions = None) -> None:
        """Saves a manifest in JSON format.

        Args:
            manifest (PyMigrationManifest): The manifest to save.
            path (str): The file path to save the manifest to.
            cancel (_type_, optional): A cancellation token to obey. Defaults to tableau_migration.cancellation_token.
            json_options (JsonSerializerOptions, optional): Optional JSON options to use. Defaults to None.

        Returns:
            None
        """
        _manifest_obj = manifest._migration_manifest
        if(json_options is None):
            return self._migration_manifest_serializer.SaveAsync(_manifest_obj,path,cancel).GetAwaiter().GetResult()
        else:
            return self._migration_manifest_serializer.SaveAsync(manifest._migration_manifest,path,cancel,json_options).GetAwaiter().GetResult()
    
    def load(self, path: str, cancel = tableau_migration.cancellation_token, json_options: JsonSerializerOptions = None) -> PyMigrationManifest:
        """Loads a manifest from JSON format.

        Args:
            path (str): The file path to load the manifest from.
            cancel (_type_, optional): A cancellation token to obey. Defaults to tableau_migration.cancellation_token.
            json_options (JsonSerializerOptions, optional): Optional JSON options to use. Defaults to None.

        Returns:
            PyMigrationManifest: The PyMigrationManifest, or null if the manifest could not be loaded.
        """
        return PyMigrationManifest(self._migration_manifest_serializer.LoadAsync(path,cancel,json_options).GetAwaiter().GetResult())
        