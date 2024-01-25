# Migration SDK - Test Application

## Getting Started

* Make sure the "Python" workload is installed for Visual Studio;
* Install Python3 (and pip3) (make sure it's on PATH) and restart Visual Studio;
	* It MUST be python3. To verify, start `python` and check the version. Python2 is very different.
* Update PIP and SetupTools to the latest version by executing `python -m pip install --upgrade pip setuptools`
* Create the python virtual environment
  * (From migration-sdk/tests/Python.TestApplication): `python -m venv env --system-site-packages --upgrade-deps`
* Activate the virtual environment:
  * `.\env\Scripts\Activate.ps1`
* Install the required modules:
  * `python -m pip install -r .\requirements.txt`
* Set the configuration (on config.ini or config.DEV.ini):
  * [SOURCE] -> URL: The origin Tableau Server URL;
  * [SOURCE] -> ACCESS_TOKEN_NAME: The origin Tableau Server Token Name;
  * [SOURCE] -> ACCESS_TOKEN: The origin Tableau Server Token;
  * [DESTINATION] -> URL: The destination Tableau Cloud URL;
  * [DESTINATION] -> SITE_CONTENT_URL: The destination Tableau Cloud Site;
  * [DESTINATION] -> ACCESS_TOKEN_NAME: The destination Tableau Cloud Site Token Name;
  * [DESTINATION] -> ACCESS_TOKEN: The destination Tableau Cloud Site Token;
  * [MIGRATION_OPTIONS] -> BASE_OVERRIDE_MAIL_ADDRESS: The base email address used to migrate the users without email;
* Execute the application:
  * `python .\main.py`
