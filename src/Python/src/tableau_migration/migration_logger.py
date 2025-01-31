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

"""Migration Logger implementation."""
import logging
from Tableau.Migration.Interop.Logging import NonGenericLoggerBase

class MigrationLogger(NonGenericLoggerBase):
    """Migration Logger implementation."""
    __namespace__ = "Tableau.Migration.Interop.Logging"

    def __init__(self, name) -> None:
        """Default init.

        Args:
            name: Name of the logger
        
        Returns: None.
        """
        logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s', level=logging.INFO)
        self._logger = logging.getLogger(name)


    def Log(self, log_level, event_id, state, exception, message):  # noqa: N802 - Must be named like this for dotnet inheritance to work
        """Writes a log entry with a pre-formatted state object.

        https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger.log?view=dotnet-plat-ext-7.0

        Args:
            log_level: Entry will be written on this level.
            event_id: Id of the event.
            state: The pre-formatted entry to be written.
            exception: The exception related to this entry.
            message: The pre-formatted message to write.
        """
        if(self.IsEnabled(log_level) is False):
            return # shortcut

        # Python log level are 10x the C# level, multiplying here and passing along the message
        # Python: https://docs.python.org/3/howto/logging.html#logging-levels
        # C#: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel
        self._logger.log(int(log_level) * 10, message)


    def IsEnabled(self, log_level): # noqa: N802 - Must be named like this for dotnet inheritance to work
        """Checks if the given log level" is enabled.

        Args:
            log_level: Level to be checked.

        Returns: True if enabled
        """
        return (int(log_level) * 10 >= self._logger.level)
