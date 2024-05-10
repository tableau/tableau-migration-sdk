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


import pytest
import os

# The conftest.py file serves as a means of providing fixtures for an entire directory. 
# Fixtures defined in a conftest.py can be used by any test in that package without needing to import them (pytest will automatically discover them).
# https://docs.pytest.org/en/7.1.x/reference/fixtures.html#conftest-py-sharing-fixtures-across-multiple-files

@pytest.fixture(autouse=False)
def skip_by_python_lifetime_env_var():
    if os.getenv('SKIP_PYTHON_LIFETIME_TESTS', 'False').lower() == 'true':
        pytest.skip("skipped due to SKIP_PYTHON_LIFETIME_TESTS environment variable")
