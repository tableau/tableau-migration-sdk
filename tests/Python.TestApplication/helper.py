import sys
from os.path import abspath
from pathlib import Path

# Builds the C# project. This is here so we can make changes to the C# and python code without packaging
import build                                                                                                # noqa: F401

_main_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../../src/Python/src")
sys.path.append(_main_module_path)

import tableau_migration                                                                                    # noqa: F401
import clr                                                                                                  # noqa: F401

clr.AddReference("Tableau.Migration.TestComponents")

_manifestPath = abspath(Path(__file__).parent.resolve().__str__() + "/manifest.json")
sys.path.append(_manifestPath)
