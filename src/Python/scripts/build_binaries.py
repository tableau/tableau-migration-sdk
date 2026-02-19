# noqa: D100
# This is a script to build Tableau.Migration dlls before running the Python Wrapper import.
import subprocess
import shutil

# Build the solution so we have the published binaries.
from os.path import abspath
migration_project = abspath("../../Tableau.Migration/Tableau.Migration.csproj")

# Build binaries and put them on the path.
bin_path = abspath("../src/tableau_migration/bin")

shutil.rmtree(bin_path, True)
subprocess.run(["dotnet", "publish", migration_project, "-o", bin_path, "-f", "net8.0"])