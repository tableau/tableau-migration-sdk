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

### 
# WARNING
# 
# Careful loading dotnet objects globally
# ex: from Tableau.Migration import IMigrationPlanBuilder
#
# If you don't have the binaries build and in the right location, and you accidently 
# import from the wrong namespace, then the loading of this file will fail.
# That in turns means the Test Explorer won't display the tests
# If this happens, always check the "Test Output" for errors
###

import pytest
import sys
from os.path import abspath
from pathlib import Path
import inspect
from enum import Enum
from typing import List

from tableau_migration.migration import (
    PyMigrationResult,
    PyMigrationCompletionStatus,
    PyMigrationManifest,
    PyResult)

from tableau_migration.migration_engine import (
    PyMigrationPlanBuilder,
    PyServerToCloudMigrationPlanBuilder,
    PyMigrationPlan)

from tableau_migration.migration_engine_endpoints_search import (
    PyDestinationContentReferenceFinder,
    PyDestinationContentReferenceFinderFactory,
    PySourceContentReferenceFinder,
    PySourceContentReferenceFinderFactory)

from tableau_migration.migration_engine_hooks import (
    PyMigrationHookBuilder,
    PyMigrationHookFactoryCollection)

from tableau_migration.migration_engine_hooks_transformers import (
    PyContentTransformerBuilder)

from tableau_migration.migration_engine_hooks_mappings import (
    PyContentMappingBuilder)

from tableau_migration.migration_engine_hooks_filters import (
    PyContentFilterBuilder)

from tableau_migration.migration_engine_options import (
    PyMigrationPlanOptionsBuilder,
    PyMigrationPlanOptionsCollection)

from tableau_migration.migration_engine_manifest import (
    PyMigrationManifestSerializer)

from tableau_migration.migration_engine_migrators import (
    PyMigrator)

from Tableau.Migration import (
    MigrationCompletionStatus)

_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../src/tableau_migration")
sys.path.append(_module_path)

def get_class_methods(cls) -> List[str]:
    """Gets all the methods in a class.

    https://stackoverflow.com/a/4241225.
    """
    _base_object = dir(type('dummy', (object,), {}))
    methods = [item[0] for item in inspect.getmembers(cls, predicate=inspect.ismethod)
                if item[0] not in _base_object]
    methods.extend([item[0] for item in inspect.getmembers(cls, predicate=inspect.isfunction) if item[0] not in _base_object])

    #Remove python internal methods
    return [m for m in methods if not m.startswith("__")]


def get_class_properties(cls) -> List[str]:
    """Gets all the properties in a class.

    https://stackoverflow.com/a/34643176.
    """
    return [item[0] for item in inspect.getmembers(cls) 
                if isinstance(item[1], property)]


def get_enum(enumType: Enum):
    """Returns list of enum name and value"""
    return [(item.name, item.value) for item in enumType]

def normalize_name(name: str) -> str:
    return name.replace("_", "").lower()

def compare_names(dotnet_names: List[str], python_names: List[str], ) -> str:
    """Compares dotnet names with python names

    dotnet names look like 'DoWalk'
    python names look like 'do_walk'

    Also, dotnet may end end Async, which is removed for python.
    The expectation is that dotnet does not supply sync method for async methods
    """
    dotnet_lookup = {}
    python_lookup = {}

    dotnet_normalized = []
    python_normalized = []

    # normalize the names and create a lookup table for friendly output
    for item in dotnet_names:
        # Python does not support await/async so all method need to be syncronous
        # The wrapper method is expected to handle this, so the "Async" suffix should be dropped
        normalized_name = normalize_name(item.removesuffix("Async"))
        dotnet_normalized.append(normalized_name)
        dotnet_lookup[normalized_name] = item

    for item in python_names:
        normalized_name = normalize_name(item)
        python_normalized.append(normalized_name)
        python_lookup[normalized_name] = item

    # Count number of unique occurances
    python_set = set(python_normalized) # removes dupes
    dotnet_set = set(dotnet_normalized) # removes dupes

    message = ""

    # Check what names are missing from python but are available in dotnet
    python_lacks = [x for x in dotnet_set if x not in python_set and "get" + x not in python_set and "set" not in python_set]
    lacks_message_items = [f"({lacks_item}: (py:???->net:{dotnet_lookup[lacks_item]}))" for lacks_item in python_lacks]
    message += f"Python lacks elements {lacks_message_items}\n" if lacks_message_items else ''

    # Check what names are extra in python and missing from dotnet
    python_extra = [x for x in python_set if x not in dotnet_set and x.removeprefix("get") not in dotnet_set and x.removeprefix("set") not in dotnet_set]
    extra_message_items = [f"({extra_item}: (py:{python_lookup[extra_item]}->net:???))" for extra_item in python_extra]
    message += f"Python has extra elements {extra_message_items}\n" if extra_message_items else ''

    return message

def compare_lists(expected, actual) -> str:
    """https://stackoverflow.com/a/61494686."""
    seen = set()
    duplicates = list()
    for x in actual:
        if x in seen:
            duplicates.append(x)
        else:
            seen.add(x)

    lacks = set(expected) - set(actual)
    extra = set(actual) - set(expected)
    message = f"Lacks elements {lacks} " if lacks else ''
    message += f"Extra elements {extra} " if extra else ''
    message += f"Duplicate elements {duplicates}" if duplicates else ''
    return message

def verify_enum(python_enum, dotnet_enum):
    """Verify that dotnet and python enum are the same

    Currently this is only verified working for 'int' type enums. But should be easily
    modified to support more types if needed
    """
    from Tableau.Migration.Interop import InteropHelper

    dotnet_enum = [(normalize_name(item.Item1), item.Item2) for item in InteropHelper.GetEnum[dotnet_enum]()]
    python_enum = [(normalize_name(item[0]), item[1]) for item in get_enum(python_enum)]

    message = compare_lists(dotnet_enum, python_enum)
    assert not message

class TestNameComparison():
    def test_valid(self):
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync", "Static"]
        python_names = ["do_walk", "run", "run", "get_static", "set_static"]

        message = compare_names(dotnet_names, python_names)
        assert not message

    def test_python_extra(self):
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync"]
        python_names = ["do_walk", "run", "run", "crawl"]

        message = compare_names(dotnet_names, python_names)
        assert message == "Python has extra elements ['(crawl: (py:crawl->net:???))']\n" 

    def test_dotnet_extra(self): # meaning python is missing it
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync", "Id"]
        python_names = ["run", "run", "id"]

        message = compare_names(dotnet_names, python_names)
        assert message == "Python lacks elements ['(dowalk: (py:???->net:DoWalk))']\n"

    def test_overloaded_missing(self):
        # Python does not support multiple method with the same name
        # So we need to be smart about how we call them. 
        # This test makes sure that overloaded methods show up at least once in python
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync"]
        python_names = ["do_walk", "run"]

        message = compare_names(dotnet_names, python_names)
        assert not message

# region _generated

from tableau_migration.migration import (  # noqa: E402, F401
    PyContentLocation,
    PyContentReference,
    PyEmptyIdContentReference,
    PyMigrationCompletionStatus,
    PyPipelineProfile,
    PyResult
)

from tableau_migration.migration_api_rest import PyRestIdentifiable # noqa: E402, F401

from tableau_migration.migration_api_rest_models import (  # noqa: E402, F401
    PyAdministratorLevels,
    PyContentPermissions,
    PyExtractEncryptionModes,
    PyLabelCategories,
    PyLicenseLevels,
    PyPermissionsCapabilityModes,
    PyPermissionsCapabilityNames,
    PySiteRoles
)

from tableau_migration.migration_api_rest_models_types import (  # noqa: E402, F401
    PyAuthenticationTypes,
    PyDataSourceFileTypes,
    PyWorkbookFileTypes
)

from tableau_migration.migration_content import (  # noqa: E402, F401
    PyCloudSubscription,
    PyConnection,
    PyConnectionsContent,
    PyContainerContent,
    PyCustomView,
    PyDataSource,
    PyDataSourceDetails,
    PyDescriptionContent,
    PyExtractContent,
    PyFavorite,
    PyFavoriteContentType,
    PyGroup,
    PyGroupSet,
    PyGroupUser,
    PyLabel,
    PyProject,
    PyPublishableCustomView,
    PyPublishableDataSource,
    PyPublishableGroup,
    PyPublishableGroupSet,
    PyPublishableWorkbook,
    PyPublishedContent,
    PyServerSubscription,
    PySubscription,
    PySubscriptionContent,
    PyTag,
    PyUser,
    PyUserAuthenticationType,
    PyUsernameContent,
    PyView,
    PyWithDomain,
    PyWithOwner,
    PyWithTags,
    PyWithWorkbook,
    PyWorkbook,
    PyWorkbookDetails
)

from tableau_migration.migration_content_permissions import (  # noqa: E402, F401
    PyCapability,
    PyGranteeCapability,
    PyGranteeType,
    PyPermissions,
    PyPermissionSet
)

from tableau_migration.migration_content_schedules import (  # noqa: E402, F401
    PyExtractRefreshContentType,
    PyExtractRefreshTask,
    PyFrequencyDetails,
    PyInterval,
    PySchedule,
    PyWithSchedule
)

from tableau_migration.migration_content_schedules_cloud import PyCloudSchedule # noqa: E402, F401

from tableau_migration.migration_content_schedules_server import PyServerSchedule # noqa: E402, F401

from tableau_migration.migration_content_search import PyContentReferenceFinder # noqa: E402, F401

from tableau_migration.migration_engine import PyContentMigrationItem # noqa: E402, F401

from tableau_migration.migration_engine_actions import PyMigrationActionResult # noqa: E402, F401

from tableau_migration.migration_engine_hooks_mappings import PyContentMappingContext # noqa: E402, F401

from tableau_migration.migration_engine_hooks_postpublish import (  # noqa: E402, F401
    PyBulkPostPublishContext,
    PyContentItemPostPublishContext
)

from tableau_migration.migration_engine_manifest import (  # noqa: E402, F401
    PyMigrationManifestEntry,
    PyMigrationManifestEntryEditor,
    PyMigrationManifestEntryStatus
)

from tableau_migration.migration_engine_migrators import PyContentItemMigrationResult # noqa: E402, F401

from tableau_migration.migration_engine_migrators_batch import PyContentBatchMigrationResult # noqa: E402, F401

from tableau_migration.migration_engine_pipelines import (  # noqa: E402, F401
    PyMigrationPipelineContentType,
    PyServerToCloudMigrationPipeline
)


from Tableau.Migration import MigrationCompletionStatus
from Tableau.Migration import PipelineProfile
from Tableau.Migration.Api.Rest.Models import AdministratorLevels
from Tableau.Migration.Api.Rest.Models import ContentPermissions
from Tableau.Migration.Api.Rest.Models import ExtractEncryptionModes
from Tableau.Migration.Api.Rest.Models import LabelCategories
from Tableau.Migration.Api.Rest.Models import LicenseLevels
from Tableau.Migration.Api.Rest.Models import PermissionsCapabilityModes
from Tableau.Migration.Api.Rest.Models import PermissionsCapabilityNames
from Tableau.Migration.Api.Rest.Models import SiteRoles
from Tableau.Migration.Api.Rest.Models.Types import AuthenticationTypes
from Tableau.Migration.Api.Rest.Models.Types import DataSourceFileTypes
from Tableau.Migration.Api.Rest.Models.Types import WorkbookFileTypes
from Tableau.Migration.Content import FavoriteContentType
from Tableau.Migration.Content.Permissions import GranteeType
from Tableau.Migration.Content.Schedules import ExtractRefreshContentType
from Tableau.Migration.Engine.Manifest import MigrationManifestEntryStatus

_generated_class_data = {
    PyContentLocation: (PyContentLocation, [ "ForContentType" ], []),
    PyContentReference: (PyContentReference, None, []),
    PyEmptyIdContentReference: (PyEmptyIdContentReference, None, []),
    PyResult: (PyResult, [ "CastFailure" ], []),
    PyRestIdentifiable: (PyRestIdentifiable, None, []),
    PyCloudSubscription: (PyCloudSubscription, None, []),
    PyConnection: (PyConnection, None, []),
    PyConnectionsContent: (PyConnectionsContent, None, []),
    PyContainerContent: (PyContainerContent, None, []),
    PyCustomView: (PyCustomView, None, []),
    PyDataSource: (PyDataSource, [ "SetLocation" ], []),
    PyDataSourceDetails: (PyDataSourceDetails, [ "SetLocation" ], []),
    PyDescriptionContent: (PyDescriptionContent, None, []),
    PyExtractContent: (PyExtractContent, None, []),
    PyFavorite: (PyFavorite, None, []),
    PyGroup: (PyGroup, [ "SetLocation" ], []),
    PyGroupSet: (PyGroupSet, [ "SetLocation" ], []),
    PyGroupUser: (PyGroupUser, None, []),
    PyLabel: (PyLabel, None, []),
    PyProject: (PyProject, [ "Container", "SetLocation" ], []),
    PyPublishableCustomView: (PyPublishableCustomView, [ "DisposeAsync", "File" ], []),
    PyPublishableDataSource: (PyPublishableDataSource, [ "DisposeAsync", "File", "SetLocation" ], []),
    PyPublishableGroup: (PyPublishableGroup, [ "SetLocation" ], []),
    PyPublishableGroupSet: (PyPublishableGroupSet, [ "SetLocation" ], []),
    PyPublishableWorkbook: (PyPublishableWorkbook, [ "ChildPermissionContentItems", "ChildType", "DisposeAsync", "File", "SetLocation", "ShouldMigrateChildPermissions" ], []),
    PyPublishedContent: (PyPublishedContent, None, []),
    PyServerSubscription: (PyServerSubscription, None, []),
    PySubscription: (PySubscription, None, []),
    PySubscriptionContent: (PySubscriptionContent, None, []),
    PyTag: (PyTag, None, []),
    PyUser: (PyUser, [ "SetLocation" ], []),
    PyUserAuthenticationType: (PyUserAuthenticationType, None, []),
    PyUsernameContent: (PyUsernameContent, [ "SetLocation" ], []),
    PyView: (PyView, None, []),
    PyWithDomain: (PyWithDomain, None, []),
    PyWithOwner: (PyWithOwner, None, []),
    PyWithTags: (PyWithTags, None, []),
    PyWithWorkbook: (PyWithWorkbook, None, []),
    PyWorkbook: (PyWorkbook, [ "SetLocation" ], []),
    PyWorkbookDetails: (PyWorkbookDetails, [ "ChildPermissionContentItems", "ChildType", "SetLocation", "ShouldMigrateChildPermissions" ], []),
    PyCapability: (PyCapability, None, []),
    PyGranteeCapability: (PyGranteeCapability, None, []),
    PyPermissions: (PyPermissions, None, []),
    PyPermissionSet: (PyPermissionSet, None, []),
    PyExtractRefreshTask: (PyExtractRefreshTask, None, []),
    PyFrequencyDetails: (PyFrequencyDetails, None, []),
    PyInterval: (PyInterval, None, []),
    PySchedule: (PySchedule, None, []),
    PyWithSchedule: (PyWithSchedule, None, []),
    PyCloudSchedule: (PyCloudSchedule, None, []),
    PyServerSchedule: (PyServerSchedule, [ "ExtractRefreshTasks" ], []),
    PyContentReferenceFinder: (PyContentReferenceFinder, None, []),
    PyContentMigrationItem: (PyContentMigrationItem, None, []),
    PyMigrationActionResult: (PyMigrationActionResult, [ "CastFailure" ], []),
    PyContentMappingContext: (PyContentMappingContext, [ "ToTask" ], []),
    PyBulkPostPublishContext: (PyBulkPostPublishContext, [ "ToTask" ], []),
    PyContentItemPostPublishContext: (PyContentItemPostPublishContext, [ "ToTask" ], []),
    PyMigrationManifestEntry: (PyMigrationManifestEntry, None, []),
    PyMigrationManifestEntryEditor: (PyMigrationManifestEntryEditor, [ "SetFailed" ], []),
    PyContentItemMigrationResult: (PyContentItemMigrationResult, [ "CastFailure" ], []),
    PyContentBatchMigrationResult: (PyContentBatchMigrationResult, [ "CastFailure" ], []),
    PyMigrationPipelineContentType: (PyMigrationPipelineContentType, [ "GetContentTypeForInterface", "GetPostPublishTypesForInterface", "GetPublishTypeForInterface", "WithPrepareType", "WithPublishType", "WithResultType" ], []),
    PyServerToCloudMigrationPipeline: (PyServerToCloudMigrationPipeline, [ "BuildActions", "BuildPipeline", "CreateDestinationCache", "CreateSourceCache", "GetBatchMigrator", "GetDestinationLockedProjectCache", "GetItemConverter", "GetItemPreparer", "GetMigrator" ], [])
}

_generated_enum_data = [
    (PyMigrationCompletionStatus, MigrationCompletionStatus),
    (PyPipelineProfile, PipelineProfile),
    (PyAdministratorLevels, AdministratorLevels),
    (PyContentPermissions, ContentPermissions),
    (PyExtractEncryptionModes, ExtractEncryptionModes),
    (PyLabelCategories, LabelCategories),
    (PyLicenseLevels, LicenseLevels),
    (PyPermissionsCapabilityModes, PermissionsCapabilityModes),
    (PyPermissionsCapabilityNames, PermissionsCapabilityNames),
    (PySiteRoles, SiteRoles),
    (PyAuthenticationTypes, AuthenticationTypes),
    (PyDataSourceFileTypes, DataSourceFileTypes),
    (PyWorkbookFileTypes, WorkbookFileTypes),
    (PyFavoriteContentType, FavoriteContentType),
    (PyGranteeType, GranteeType),
    (PyExtractRefreshContentType, ExtractRefreshContentType),
    (PyMigrationManifestEntryStatus, MigrationManifestEntryStatus)
]

# endregion

_test_class_data = {
    PyMigrationPlanBuilder: (PyMigrationPlanBuilder, [ "ForCustomPipeline", "ForCustomPipelineFactory" ], []),
    PyServerToCloudMigrationPlanBuilder: (PyServerToCloudMigrationPlanBuilder, [ "ForCustomPipeline", "ForCustomPipelineFactory" ], []),
    PyMigrationResult: (PyMigrationResult, None, []),
    PyMigrationManifest: (PyMigrationManifest, None, []),
    PyMigrator: (PyMigrator, None, []),
    PyMigrationPlan: (PyMigrationPlan, [ "PipelineFactoryOverride" ], []),
    PyMigrationHookBuilder: (PyMigrationHookBuilder, None, []),
    PyContentTransformerBuilder: (PyContentTransformerBuilder, None, []),
    PyContentMappingBuilder: (PyContentMappingBuilder, None, []),
    PyContentFilterBuilder: (PyContentFilterBuilder, None, []),
    PyMigrationPlanOptionsBuilder: (PyMigrationPlanOptionsBuilder, None, []),
    PyMigrationPlanOptionsCollection: (PyMigrationPlanOptionsCollection, None, []),
    PyMigrationHookFactoryCollection: (PyMigrationHookFactoryCollection, None, []),
    PyDestinationContentReferenceFinder: (PyDestinationContentReferenceFinder, None, []),
    PyDestinationContentReferenceFinderFactory: (PyDestinationContentReferenceFinderFactory, [ "ForContentType" ], []),
    PySourceContentReferenceFinder: (PySourceContentReferenceFinder, None, []),
    PySourceContentReferenceFinderFactory: (PySourceContentReferenceFinderFactory, [ "ForContentType" ], []),
    PyMigrationManifestSerializer: (PyMigrationManifestSerializer, None, []),
}
_test_class_data.update(_generated_class_data)

# Known factory extensions
_test_class_data[PyCapability][2].append("create")
_test_class_data[PyGranteeCapability][2].append("create")

@pytest.mark.parametrize("python_class, ignored_members, ignored_extra_members", _test_class_data.values())
def test_classes(python_class, ignored_members, ignored_extra_members):
    """Verify that all the python wrapper classes actually wrap all the dotnet methods and properties."""
    from Tableau.Migration.Interop import InteropHelper

    # Verify that this class has a _dotnet_base.
    assert python_class._dotnet_base

    dotnet_class = python_class._dotnet_base

    
    # Get all the python methods and properties.
    _all_py_methods = get_class_methods(python_class)
    print("All Python methods")
    print(_all_py_methods)

    _all_py_props = get_class_properties(python_class)
    print("All Python properties")
    print(_all_py_props)

    if ignored_extra_members is None:
        _clean_py_methods = _all_py_methods
        _clean_py_props = _all_py_props
    else:
        _clean_py_methods = [method for method in _all_py_methods if method not in ignored_extra_members]
        _clean_py_props = [prop for prop in _all_py_props if prop not in ignored_extra_members]

    # Get all the dotnet methods and field/properties.
    _all_dotnet_methods = [m for m in InteropHelper.GetMethods(dotnet_class)]
    _all_dotnet_props = [p for p in InteropHelper.GetProperties(dotnet_class)] + [f for f in InteropHelper.GetFields(dotnet_class)]

    # Clean methods that should be ignored.
    if ignored_members is None:
        _clean_dotnet_methods = _all_dotnet_methods
        _clean_dotnet_props = _all_dotnet_props
    else:
        _clean_dotnet_methods = [method for method in _all_dotnet_methods if method not in ignored_members]    
        print("Clean .net methods")
        print(_clean_dotnet_methods)

        _clean_dotnet_props = [prop for prop in _all_dotnet_props if prop not in ignored_members]
        print("Clean .net properties")
        print(_clean_dotnet_props)

    # Compare the lists.
    member_message = compare_names(_clean_dotnet_methods + _clean_dotnet_props, _clean_py_methods + _clean_py_props)

    # Assert
    assert not member_message # Remember that the names are normalized.

_test_enum_data = []
_test_enum_data.extend(_generated_enum_data)

@pytest.mark.parametrize("python_class, dotnet_enum", _test_enum_data)
def test_enum(python_class, dotnet_enum):
    verify_enum(python_class, dotnet_enum)
