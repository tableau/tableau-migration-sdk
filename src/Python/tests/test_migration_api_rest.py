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

# region _generated

from uuid import UUID # noqa: E402, F401

from System import Guid # noqa: E402, F401
from Tableau.Migration.Api.Rest import IRestIdentifiable # noqa: E402, F401

from tableau_migration.migration_api_rest import PyRestIdentifiable # noqa: E402, F401
# Extra imports for tests.
from tests.helpers.autofixture import AutoFixtureTestBase # noqa: E402, F401

class TestPyRestIdentifiableGenerated(AutoFixtureTestBase):
    
    def test_ctor(self):
        dotnet = self.create(IRestIdentifiable)
        py = PyRestIdentifiable(dotnet)
        assert py._dotnet == dotnet
    
    def test_id_getter(self):
        dotnet = self.create(IRestIdentifiable)
        py = PyRestIdentifiable(dotnet)
        assert py.id == None if dotnet.Id is None else UUID(dotnet.Id.ToString())
    

# endregion

