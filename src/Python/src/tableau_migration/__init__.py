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

"""Tableau Migration SDK package that wraps the dotnet implementation."""
import sys
from pathlib import Path
from os.path import abspath

# Append current file path to PATH
sys.path.append(Path(__file__).parent.resolve().__str__())

_bin_path = abspath(Path(__file__).parent.resolve().__str__() + "/bin")
sys.path.append(_bin_path)

# noqu: E402 stops the linter from flagging global imports that are not at the top of the file
# The python net and dotnet import must happen in this order else they won't load correctly
# See: https://pythonnet.github.io/pythonnet/python.html#loading-a-runtime

# Load python net and the clr
from pythonnet import load  # noqa: E402 
# load must be called before import clr
load("coreclr")
import clr  # noqa: E402

clr.AddReference("Tableau.Migration")
clr.AddReference("Microsoft.Extensions.DependencyInjection")

import tableau_migration.migration # noqa: E402
_services = None
_service_collection = None
_logger_names = [] # List of logger names

tableau_migration.migration._initialize()

# Create out global default cancellation token
from System.Threading import CancellationTokenSource # noqa: E402 
cancellation_token_source = CancellationTokenSource()
cancellation_token = cancellation_token_source.Token

# Friendly renames for common top-level imports
from tableau_migration.migration import PyMigrationResult as MigrationResult # noqa: E402, F401
from tableau_migration.migration_engine import PyMigrationPlanBuilder as MigrationPlanBuilder # noqa: E402, F401
from tableau_migration.migration_engine_hooks_filters_interop import PyContentFilterBase as ContentFilterBase # noqa: E402, F401
from tableau_migration.migration_engine_hooks_interop import ( # noqa: E402, F401
    PyContentBatchMigrationCompletedHookBase as ContentBatchMigrationCompletedHookBase,
    PyInitializeMigrationHookBase as InitializeMigrationHookBase,
    PyMigrationActionCompletedHookBase as MigrationActionCompletedHookBase
)
from tableau_migration.migration_engine_hooks_mappings_interop import ( # noqa: E402, F401
    PyContentMappingBase as ContentMappingBase,
    PyTableauCloudUsernameMappingBase as TableauCloudUsernameMappingBase
)
from tableau_migration.migration_engine_hooks_postpublish_interop import ( # noqa: E402, F401
    PyBulkPostPublishHookBase as BulkPostPublishHookBase,
    PyContentItemPostPublishHookBase as ContentItemPostPublishHookBase
)
from tableau_migration.migration_engine_hooks_results import PyInitializeMigrationHookResult as IInitializeMigrationHookResult # noqa: E402, F401
from tableau_migration.migration_engine_hooks_transformers_interop import ( # noqa: E402, F401
    PyContentTransformerBase as ContentTransformerBase,
    PyXmlContentTransformerBase as XmlContentTransformerBase
)
from tableau_migration.migration_engine_migrators import PyMigrator as Migrator # noqa: E402, F401
from tableau_migration.migration_engine_endpoints_search import PyDestinationContentReferenceFinder as IDestinationContentReferenceFinder # noqa: E402, F401
from tableau_migration.migration_engine_endpoints_search import PyDestinationContentReferenceFinderFactory as IDestinationContentReferenceFinderFactory # noqa: E402, F401
from tableau_migration.migration_engine_endpoints_search import PySourceContentReferenceFinder as ISourceContentReferenceFinder # noqa: E402, F401
from tableau_migration.migration_engine_endpoints_search import PySourceContentReferenceFinderFactory as ISourceContentReferenceFinderFactory # noqa: E402, F401
from tableau_migration.migration_content_schedules_cloud import PyCloudExtractRefreshTask as ICloudExtractRefreshTask # noqa: E402, F401
from tableau_migration.migration_content_schedules_server import PyServerExtractRefreshTask as IServerExtractRefreshTask # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifest as MigrationManifest # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifestSerializer as MigrationManifestSerializer # noqa: E402, F401

# region _generated

from tableau_migration.migration import PyContentLocation as ContentLocation # noqa: E402, F401
from tableau_migration.migration import PyContentReference as IContentReference # noqa: E402, F401
from tableau_migration.migration import PyMigrationCompletionStatus as MigrationCompletionStatus # noqa: E402, F401
from tableau_migration.migration import PyResult as IResult # noqa: E402, F401
from tableau_migration.migration_api_rest import PyRestIdentifiable as IRestIdentifiable # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PyAdministratorLevels as AdministratorLevels # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PyContentPermissions as ContentPermissions # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PyExtractEncryptionModes as ExtractEncryptionModes # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PyLabelCategories as LabelCategories # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PyLicenseLevels as LicenseLevels # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PyPermissionsCapabilityModes as PermissionsCapabilityModes # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PyPermissionsCapabilityNames as PermissionsCapabilityNames # noqa: E402, F401
from tableau_migration.migration_api_rest_models import PySiteRoles as SiteRoles # noqa: E402, F401
from tableau_migration.migration_api_rest_models_types import PyAuthenticationTypes as AuthenticationTypes # noqa: E402, F401
from tableau_migration.migration_api_rest_models_types import PyDataSourceFileTypes as DataSourceFileTypes # noqa: E402, F401
from tableau_migration.migration_api_rest_models_types import PyWorkbookFileTypes as WorkbookFileTypes # noqa: E402, F401
from tableau_migration.migration_content import PyConnection as IConnection # noqa: E402, F401
from tableau_migration.migration_content import PyConnectionsContent as IConnectionsContent # noqa: E402, F401
from tableau_migration.migration_content import PyContainerContent as IContainerContent # noqa: E402, F401
from tableau_migration.migration_content import PyCustomView as ICustomView # noqa: E402, F401
from tableau_migration.migration_content import PyDataSource as IDataSource # noqa: E402, F401
from tableau_migration.migration_content import PyDataSourceDetails as IDataSourceDetails # noqa: E402, F401
from tableau_migration.migration_content import PyDescriptionContent as IDescriptionContent # noqa: E402, F401
from tableau_migration.migration_content import PyExtractContent as IExtractContent # noqa: E402, F401
from tableau_migration.migration_content import PyGroup as IGroup # noqa: E402, F401
from tableau_migration.migration_content import PyGroupUser as IGroupUser # noqa: E402, F401
from tableau_migration.migration_content import PyLabel as ILabel # noqa: E402, F401
from tableau_migration.migration_content import PyProject as IProject # noqa: E402, F401
from tableau_migration.migration_content import PyPublishableCustomView as IPublishableCustomView # noqa: E402, F401
from tableau_migration.migration_content import PyPublishableDataSource as IPublishableDataSource # noqa: E402, F401
from tableau_migration.migration_content import PyPublishableGroup as IPublishableGroup # noqa: E402, F401
from tableau_migration.migration_content import PyPublishableWorkbook as IPublishableWorkbook # noqa: E402, F401
from tableau_migration.migration_content import PyPublishedContent as IPublishedContent # noqa: E402, F401
from tableau_migration.migration_content import PyTag as ITag # noqa: E402, F401
from tableau_migration.migration_content import PyUser as IUser # noqa: E402, F401
from tableau_migration.migration_content import PyUsernameContent as IUsernameContent # noqa: E402, F401
from tableau_migration.migration_content import PyView as IView # noqa: E402, F401
from tableau_migration.migration_content import PyWithDomain as IWithDomain # noqa: E402, F401
from tableau_migration.migration_content import PyWithOwner as IWithOwner # noqa: E402, F401
from tableau_migration.migration_content import PyWithTags as IWithTags # noqa: E402, F401
from tableau_migration.migration_content import PyWithWorkbook as IWithWorkbook # noqa: E402, F401
from tableau_migration.migration_content import PyWorkbook as IWorkbook # noqa: E402, F401
from tableau_migration.migration_content import PyWorkbookDetails as IWorkbookDetails # noqa: E402, F401
from tableau_migration.migration_content_permissions import PyCapability as ICapability # noqa: E402, F401
from tableau_migration.migration_content_permissions import PyGranteeCapability as IGranteeCapability # noqa: E402, F401
from tableau_migration.migration_content_permissions import PyGranteeType as GranteeType # noqa: E402, F401
from tableau_migration.migration_content_permissions import PyPermissions as IPermissions # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyExtractRefreshContentType as ExtractRefreshContentType # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyExtractRefreshTask as IExtractRefreshTask # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyFrequencyDetails as IFrequencyDetails # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyInterval as IInterval # noqa: E402, F401
from tableau_migration.migration_content_schedules import PySchedule as ISchedule # noqa: E402, F401
from tableau_migration.migration_content_schedules import PyWithSchedule as IWithSchedule # noqa: E402, F401
from tableau_migration.migration_content_schedules_cloud import PyCloudSchedule as ICloudSchedule # noqa: E402, F401
from tableau_migration.migration_content_schedules_server import PyServerSchedule as IServerSchedule # noqa: E402, F401
from tableau_migration.migration_engine import PyContentMigrationItem as ContentMigrationItem # noqa: E402, F401
from tableau_migration.migration_engine_actions import PyMigrationActionResult as IMigrationActionResult # noqa: E402, F401
from tableau_migration.migration_engine_hooks_mappings import PyContentMappingContext as ContentMappingContext # noqa: E402, F401
from tableau_migration.migration_engine_hooks_postpublish import PyBulkPostPublishContext as BulkPostPublishContext # noqa: E402, F401
from tableau_migration.migration_engine_hooks_postpublish import PyContentItemPostPublishContext as ContentItemPostPublishContext # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifestEntry as IMigrationManifestEntry # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifestEntryEditor as IMigrationManifestEntryEditor # noqa: E402, F401
from tableau_migration.migration_engine_manifest import PyMigrationManifestEntryStatus as MigrationManifestEntryStatus # noqa: E402, F401
from tableau_migration.migration_engine_migrators import PyContentItemMigrationResult as IContentItemMigrationResult # noqa: E402, F401
from tableau_migration.migration_engine_migrators_batch import PyContentBatchMigrationResult as IContentBatchMigrationResult # noqa: E402, F401
from tableau_migration.migration_engine_pipelines import PyMigrationPipelineContentType as MigrationPipelineContentType # noqa: E402, F401
from tableau_migration.migration_engine_pipelines import PyServerToCloudMigrationPipeline as ServerToCloudMigrationPipeline # noqa: E402, F401

# endregion

