import sys
import shutil
import build
from os.path import abspath
from pathlib import Path

_main_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../../../src/Python/src")
sys.path.append(_main_module_path)

_test_app_path = abspath(Path(__file__).parent.resolve().__str__() + "/..")
sys.path.append(_test_app_path)

import tableau_migration
from tableau_migration import clr

clr.AddReference("Moq")
clr.AddReference("Tableau.Migration.Tests")
clr.AddReference("Tableau.Migration.TestComponents")