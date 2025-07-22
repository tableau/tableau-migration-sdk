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

# region _generated

from tableau_migration import cancellation_token # noqa: E402, F401
from tableau_migration.migration import PyContentReference # noqa: E402, F401
from typing import Sequence # noqa: E402, F401
from uuid import UUID # noqa: E402, F401

from System import Guid # noqa: E402, F401
from System.Collections.Immutable import IImmutableList # noqa: E402, F401
from Tableau.Migration import (  # noqa: E402, F401
    TaskExtensions,
    IContentReference
)
from Tableau.Migration.Content.Search import IContentReferenceFinder # noqa: E402, F401

from tableau_migration.migration_content_search import PyContentReferenceFinder # noqa: E402, F401
# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyContentReferenceFinderGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IContentReferenceFinder)
        py = PyContentReferenceFinder(dotnet)
        assert py._dotnet == dotnet
    

# endregion

class TestPyContentReferenceFinderAsync(AutoFixtureTestBase):
    
    def test_find_all_async(self):
        dotnet = self.create(IContentReferenceFinder)
        py = PyContentReferenceFinder(dotnet)
        
        result = py.find_all()
        assert result

