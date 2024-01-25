# noqa: D100
import os
import sys
from os.path import abspath
from pathlib import Path

sys.path.append(os.path.abspath('../src/'))
sys.path.append(abspath(Path(__file__).parent.resolve()))

print("Building the C# project.")
import build_binaries # noqa: E402,F401
print("..done.")
print("Importing the tableau_migration package.")
import tableau_migration # noqa: E402,F401
print("..done.")
