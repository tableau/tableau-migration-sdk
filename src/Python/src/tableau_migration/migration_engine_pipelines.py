# Copyright (c) 2025, Salesforce, Inc.
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

"""Wrapper for classes in Tableau.Migration.Engine.Pipelines namespace."""

# region _generated

from typing import Sequence # noqa: E402, F401
from typing_extensions import Self # noqa: E402, F401

import System # noqa: E402

from Tableau.Migration.Engine.Pipelines import (  # noqa: E402, F401
    MigrationPipelineContentType,
    ServerToCloudMigrationPipeline
)

class PyMigrationPipelineContentType():
    """Object that represents a definition of a content type that a pipeline migrates."""
    
    _dotnet_base = MigrationPipelineContentType
    
    def __init__(self, migration_pipeline_content_type: MigrationPipelineContentType) -> None:
        """Creates a new PyMigrationPipelineContentType object.
        
        Args:
            migration_pipeline_content_type: A MigrationPipelineContentType object.
        
        Returns: None.
        """
        self._dotnet = migration_pipeline_content_type
        
    @classmethod
    def get_users(cls) -> Self:
        """Gets the user MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.Users is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Users)
    
    @classmethod
    def get_groups(cls) -> Self:
        """Gets the groups MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.Groups is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Groups)
    
    @classmethod
    def get_projects(cls) -> Self:
        """Gets the projects MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.Projects is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Projects)
    
    @classmethod
    def get_data_sources(cls) -> Self:
        """Gets the data sources MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.DataSources is None else PyMigrationPipelineContentType(MigrationPipelineContentType.DataSources)
    
    @classmethod
    def get_workbooks(cls) -> Self:
        """Gets the workbooks MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.Workbooks is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Workbooks)
    
    @classmethod
    def get_views(cls) -> Self:
        """Gets the views MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.Views is None else PyMigrationPipelineContentType(MigrationPipelineContentType.Views)
    
    @classmethod
    def get_server_to_cloud_extract_refresh_tasks(cls) -> Self:
        """Gets the Server to Cloud extract refresh tasks MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.ServerToCloudExtractRefreshTasks is None else PyMigrationPipelineContentType(MigrationPipelineContentType.ServerToCloudExtractRefreshTasks)
    
    @classmethod
    def get_custom_views(cls) -> Self:
        """Gets the custom views MigrationPipelineContentType."""
        return None if MigrationPipelineContentType.CustomViews is None else PyMigrationPipelineContentType(MigrationPipelineContentType.CustomViews)
    
    @property
    def content_type(self) -> System.Type:
        """The content type."""
        return self._dotnet.ContentType
    
    @property
    def publish_type(self) -> System.Type:
        """Gets the publish type."""
        return self._dotnet.PublishType
    
    @property
    def result_type(self) -> System.Type:
        """Gets the result type."""
        return self._dotnet.ResultType
    
    @property
    def types(self) -> Sequence[System.Type]:
        """Gets the types for this instance."""
        return None if self._dotnet.Types is None else list(self._dotnet.Types)
    
    def get_config_key(self) -> str:
        """Gets the config key for this content type.
        
        Returns: The config key string.
        """
        result = self._dotnet.GetConfigKey()
        return result
    
    @classmethod
    def get_config_key_for_type(cls, content_type: System.Type) -> str:
        """Gets the config key for a content type.
        
        Args:
            content_type: The content type.
        
        Returns: The config key string.
        """
        result = MigrationPipelineContentType.GetConfigKeyForType(content_type)
        return result
    
class PyServerToCloudMigrationPipeline():
    """IMigrationPipeline implementation to perform migrations from Tableau Server to Tableau Cloud."""
    
    _dotnet_base = ServerToCloudMigrationPipeline
    
    def __init__(self, server_to_cloud_migration_pipeline: ServerToCloudMigrationPipeline) -> None:
        """Creates a new PyServerToCloudMigrationPipeline object.
        
        Args:
            server_to_cloud_migration_pipeline: A ServerToCloudMigrationPipeline object.
        
        Returns: None.
        """
        self._dotnet = server_to_cloud_migration_pipeline
        
    @classmethod
    def get_content_types(cls) -> Sequence[PyMigrationPipelineContentType]:
        """Content types that are supported for migrations."""
        return None if ServerToCloudMigrationPipeline.ContentTypes is None else list((None if x is None else PyMigrationPipelineContentType(x)) for x in ServerToCloudMigrationPipeline.ContentTypes)
    

# endregion

