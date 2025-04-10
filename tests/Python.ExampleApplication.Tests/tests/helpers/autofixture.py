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

from AutoFixture import Fixture, SpecimenFactory
from AutoFixture.AutoMoq import AutoMoqCustomization

class PySimpleAutoFixtureTestBase():
    
    def setup_method(self, method):
        auto_moq_cust= AutoMoqCustomization()
        auto_moq_cust.ConfigureMembers = True
        self.fixture = Fixture().Customize(auto_moq_cust)

    def create(self, T):
        return SpecimenFactory.Create[T](self.fixture)
    
    def create_many(self, T):
        return SpecimenFactory.CreateMany[T](self.fixture)