# noqa: D100
# This is a script to build Tableau.Migration dlls before running the Python Wrapper import.
import subprocess
import shutil
import sys

# Build the solution so we have the published binaries.
from os.path import abspath
from pathlib import Path
migration_project = abspath("../../Tableau.Migration/Tableau.Migration.csproj")

# Build binaries and put them on the path.
bin_path = abspath(Path(__file__).parent.resolve().__str__() + "/bin")
sys.path.append(bin_path)

shutil.rmtree(bin_path, True)
subprocess.run(["dotnet", "publish", migration_project, "-o", bin_path, "-f", "net6.0"])