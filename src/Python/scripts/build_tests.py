# noqa: D100
# This is a testing script to check that nothing is really wrong.
import subprocess
from os.path import abspath
import build_binaries

testcomponent_project = abspath("../../tests/Tableau.Migration.TestComponents/Tableau.Migration.TestComponents.csproj")

subprocess.run(["dotnet", "publish", testcomponent_project, "-o", build_binaries.bin_path, "-f", "net6.0"])