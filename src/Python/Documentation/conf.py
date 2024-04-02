# noqa: D100
import os
import sys
from os.path import abspath
from pathlib import Path

sys.path.append(os.path.abspath('../src/'))
sys.path.append(os.path.abspath('../scripts/'))
sys.path.append(abspath(Path(__file__).parent.resolve()))
import import_modules # noqa: E402,F401
print("setting documentation configuration.")

# Configuration file for the Sphinx documentation builder.
#
# For the full list of built-in configuration values, see the documentation:
# https://www.sphinx-doc.org/en/master/usage/configuration.html


# -- General configuration ---------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#general-configuration

extensions = ['sphinx.ext.autosummary','sphinx.ext.autodoc','sphinx_markdown_builder']
autosummary_generate = True

templates_path = ['_templates']
exclude_patterns = ['_build', 'Thumbs.db', '.DS_Store']
autodoc_typehints='description'
markdown_anchor_sections=True
markdown_anchor_signatures=True
add_module_names=False
print("..done.")