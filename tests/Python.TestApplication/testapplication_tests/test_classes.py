# Copyright (c) 2024, Salesforce, Inc.
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

"""Testing classes for the Python.TestApplication."""
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
import inspect
from enum import Enum
from typing import List

from migration_testcomponents_engine_manifest import (
    PyMigrationManifestSerializer)

from migration_testcomponents_filters import(
    PySpecialUserFilter,
    PyUnlicensedUserFilter)

from migration_testcomponents_mappings import(
    PySpecialUserMapping,
    PyUnlicensedUserMapping)
from migration_testcomponents_hooks import(
    PyTimeLoggerAfterActionHook)

def get_class_methods(cls):
    """
    Gets all the methods in a class.

    https://stackoverflow.com/a/4241225.
    """
    _base_object = dir(type('dummy', (object,), {}))
    return [item[0] for item in inspect.getmembers(cls, predicate=inspect.isfunction)
                if item[0] not in _base_object]
    

def get_class_properties(cls):
    """
    Gets all the properties in a class.
    
    https://stackoverflow.com/a/34643176.
    """
    return [item[0] for item in inspect.getmembers(cls) 
                if isinstance(item[1], property)]


def get_enum(enum_type: Enum):
    """Returns list of enum name and value."""
    return [(item.name, item.value) for item in enum_type]

def normalize_name(name: str) -> str:
    """Removes _ from string.""" 
    return name.replace("_", "").lower()

def remove_suffix(input: str, suffix_to_remove: str) -> str:
    """str.removesuffix() was introduced in python 3.9."""
    if(input.endswith(suffix_to_remove)):
        return input[:-len(suffix_to_remove)]
    else:
        return input

def compare_names(dotnet_names: List[str], python_names: List[str]) -> str:
    """
    Compares dotnet names with python names.

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
        normalized_name = normalize_name(remove_suffix(item, "Async"))
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
    python_lacks = [x for x in dotnet_set if x not in python_set]
    lacks_message_items = [f"({lacks_item}: (py:???->net:{dotnet_lookup[lacks_item]}))" for lacks_item in python_lacks]
    message += f"Python lacks elements {lacks_message_items}\n" if lacks_message_items else ''

    # Check what names are extra in python and missing from dotnet
    python_extra = [x for x in python_set if x not in dotnet_set]
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
    """
    Verify that dotnet and python enum are the same.
    
    Currently this is only verified working for 'int' type enums. But should be easily
    modified to support more types if needed
    """
    from Tableau.Migration.Interop import InteropHelper

    dotnet_enum = [(item.Item1, item.Item2) for item in InteropHelper.GetEnum[dotnet_enum]()]
    python_enum = get_enum(python_enum)

    message = compare_lists(dotnet_enum, python_enum)
    assert not message

class TestNameComparison():
    """Testing class to verify the tests themselves."""
    
    def test_valid(self):
        """Verifies that method names."""
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync"]
        python_names = ["do_walk", "run", "run"]

        message = compare_names(dotnet_names, python_names)
        assert not message

    def test_python_extra(self):
        """Verifies that extra python methods are found."""
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync"]
        python_names = ["do_walk", "run", "run", "crawl"]

        message = compare_names(dotnet_names, python_names)
        assert message == "Python has extra elements ['(crawl: (py:crawl->net:???))']\n" 

    def test_dotnet_extra(self): # meaning python is missing it
        """Verifies that extra dotnet method are found."""
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync"]
        python_names = ["run", "run"]

        message = compare_names(dotnet_names, python_names)
        assert message == "Python lacks elements ['(dowalk: (py:???->net:DoWalk))']\n"

    def test_overloaded_missing(self):
        """Verifies that overloaded method are handled correctly."""
        # Python does not support multiple method with the same name
        # So we need to be smart about how we call them. 
        # This test makes sure that overloaded methods show up at least once in python
        dotnet_names = ["DoWalk", "RunAsync", "RunAsync"]
        python_names = ["do_walk", "run"]

        message = compare_names(dotnet_names, python_names)
        assert not message


@pytest.mark.parametrize("python_class, ignored_methods", [                          
                          (PyMigrationManifestSerializer, None),
                          (PySpecialUserFilter, "ExecuteAsync"),
                          (PySpecialUserMapping, None),
                          (PyUnlicensedUserMapping, None),
                          (PyUnlicensedUserFilter, "ExecuteAsync"),
                          (PyTimeLoggerAfterActionHook, None)
                          ])
def test_classes(python_class, ignored_methods):
    """Verify that all the python wrapper classes actually wrap all the dotnet methods and properties."""
    from Tableau.Migration.Interop import InteropHelper

    # Verify that this class has a _dotnet_base
    assert python_class._dotnet_base

    dotnet_class = python_class._dotnet_base
            
    # Get all the python methods and properties
    _all_py_methods = get_class_methods(python_class)
    _all_py_props = get_class_properties(python_class)
    
    # Get all the dotnet methods and properties
    _all_dotnet_methods = InteropHelper.GetMethods[dotnet_class]()
    _all_dotnet_props = InteropHelper.GetProperties[dotnet_class]()

    # Clean methods that should be ignored
    if ignored_methods is None:
        _clean_dotnet_method = _all_dotnet_methods
    else:
        _clean_dotnet_method = [method for method in _all_dotnet_methods if method not in ignored_methods]

    # Compare the lists
    method_message = compare_names(_clean_dotnet_method, _all_py_methods)
    prop_message = compare_names(_all_dotnet_props, _all_py_props)
    
    # Assert
    assert not method_message # Remember that the names are normalize
    assert not prop_message # Remember that the names are normalize
