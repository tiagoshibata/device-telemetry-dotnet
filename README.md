[![Build][build-badge]][build-url]
[![Issues][issues-badge]][issues-url]
[![Gitter][gitter-badge]][gitter-url]

Device Telemetry Overview
==========================

This service offers read access to device telemetry, full CRUD for rules, and read/write for
alarms from storage for the client via a RESTful endpoint.

## Features the microservice offers:
1. Gets a list of telemetry messages for specific parameters
1. Gets a list of alarms for specific parameters
1. Gets a single alarm
1. Modifies alarm status
1. Create/Read/Update/Delete Rules
    1. Create Rules
    1. Gets a list of rules for specific parameters
    1. Gets a single rule
    1. Modify existing rule
    1. Delete existing rule

# Dependencies
1. DocumentDB Storage
1. [Storage Adapter Webservice](https://github.com/Azure/pcs-storage-adapter-dotnet)

How to use the microservice
===========================
## Quickstart - Running the service with Docker

1. Create an instance of [Azure Document Db][documentdb-url]
1. Follow the [Storage quickstart instructions][storageadapter-url]
   for setting up the Storage Adapter microservice storage.
1. Find your DocumentDb connection string. See
   [See the tip here][azurestorageconnstring-url] if you
   need help finding it.
1. Store the "Document Db Connection string" in the [env-vars-setup](scripts)
   script, then run the script. In MacOS/Linux the environment variables
   need to be set in the same session where you run Docker Compose,
   every time a new session is created.
1. [Install Docker Compose][docker-compose-install-url]
1. Start the Telemetry service using docker compose:
   ```
   cd scripts
   cd docker
   docker-compose up
   ```
1. Use an HTTP client such as [Postman][postman-url], to exercise the
   [RESTful API][project-wiki]

## Local Setup
### 1. Environment Variables

Run `scripts\env-vars-setup.cmd` on Windows or `source scripts\env-vars-setup`
on Mac/Linux to set up the environment variables needed to run the service locally.
In Windows you can also set these [in your system][windows-envvars-howto-url].

If using envornemnt variables, this service requires the following environment
variables to be set:
   1. `PCS_TELEMETRY_DOCUMENTDB_CONNSTRING` = {your Azure Document Db connection string}
   1. `PCS_STORAGEADAPTER_WEBSERVICE_URL` = http://localhost:9022/v1

## Running the service with Visual Studio

1. Install any edition of [Visual Studio 2017][vs-install-url] or Visual
   Studio for Mac. When installing check ".NET Core" workload. If you
   already have Visual Studio installed, then ensure you have
   [.NET Core Tools for Visual Studio 2017][dotnetcore-tools-url]
   installed (Windows only).
1. Create an instance of [Azure Document Db][documentdb-url]
1. Follow the [Storage quickstart instructions][storageadapter-url]
   for setting up and running the Storage Adapter microservice.
1. Open the solution in Visual Studio
1. Edit the WebService project properties, and
   define the following required environment variables. In Windows
   you can also set these [in your system][windows-envvars-howto-url].
   1. `PCS_TELEMETRY_DOCUMENTDB_CONNSTRING` = {your Azure Document Db connection string}
   1. `PCS_STORAGEADAPTER_WEBSERVICE_URL` = http://localhost:9022/v1
1. In Visual Studio, start the WebService project
1. Using an HTTP client like [Postman][postman-url],
   use the [RESTful API][project-wiki]

## Project Structure

The solution contains the following projects and folders:

* **Code** for the application is in app/com.microsoft.azure.iotsolutions.telemetry/
    * **WebService** - ASP.NET Web API exposing a RESTful API for for managing Ruels,
    Alarms, and Messages
    * **Services** - Library containing common business logic for interacting with
    storage and the StorageAdapter
* **Tests** are in the test folder
    * **WebService** - Tests for web services functionality
    * **Service** - Tests for services functionality
* **Scripts** - a folder containing scripts from the command line console,
  to build and run the solution, and other frequent tasks.
* **Routes** - defines the URL mapping to web service classes

## Build and Run from the command line

The [scripts](scripts) folder contains scripts for many frequent tasks:

* `build`: compile all the projects and run the tests.
* `compile`: compile all the projects.
* `run`: compile the projects and run the service. This will prompt for
  elevated privileges in Windows to run the web service.

## Building a customized Docker image

The `scripts` folder includes a [docker](scripts/docker) subfolder with the
scripts required to package the service into a Docker image:

* `Dockerfile`: Docker image specifications
* `build`: build a Docker image and store the image in the local registry
* `run`: run the Docker container from the image stored in the local registry
* `content`: a folder with files copied into the image, including the entry
  point script

You can also start Device Telemetry and its dependencies in one simple step,
using Docker Compose with the
[docker-compose.yml](scripts/docker/docker-compose.yml) file in the project:

```
cd scripts
cd docker
docker-compose up
```

The Docker compose configuration requires the Storage and StorageAdapter web
service URL environment variables, described previously.

## Configuration and Environment variables

The service configuration is stored using ASP.NET Core configuration
adapters, in [appsettings.ini](WebService/appsettings.ini). The INI format allows to
store values in a readable format, with comments. The application also
supports references to environment variables, which is used to import
credentials and networking details.

The configuration files in the repository reference some environment
variables that need to be created at least once. Depending on your OS and
the IDE, there are several ways to manage environment variables:

* Windows: the variables can be set [in the system][windows-envvars-howto-url]
  as a one time only task. The
  [env-vars-setup.cmd](scripts/env-vars-setup.cmd) script included needs to
  be prepared and executed just once. The settings will persist across
  terminal sessions and reboots.
* Visual Studio: the variables can be set in the project settings for WebService
  under Project Properties -> Configuration
  Properties -> Environment
* For Linux and OSX environments, the [env-vars-setup](scripts/env-vars-setup)
  script needs to be executed every time a new console is opened.
  Depending on the OS and terminal, there are ways to persist values
  globally, for more information these pages should help:
  * https://stackoverflow.com/questions/13046624/how-to-permanently-export-a-variable-in-linux
  * https://stackoverflow.com/questions/135688/setting-environment-variables-in-os-x
  * https://help.ubuntu.com/community/EnvironmentVariables

Other resources
===============

* [Telemetry service API specs](wiki/%5BAPI-Specifications%5D-Service)
* [Messages API specs](wiki/%5BAPI-Specifications%5D-Messages)
* [Alarms API specs](wiki/%5BAPI-Specifications%5D-Alarms)
* [Rules API specs](wiki/%5BAPI-Specifications%5D-Rules)

Contributing to the solution
============================

Please follow our [contribution guidelines](CONTRIBUTING.md).  We love PRs too.

Troubleshooting
===============

{TODO}

Feedback
==========

Please enter issues, bugs, or suggestions as GitHub Issues here: https://github.com/Azure/device-telemetry-dotnet/issues.

[build-badge]: https://img.shields.io/travis/Azure/device-telemetry-dotnet.svg
[build-url]: https://travis-ci.org/Azure/device-telemetry-dotnet
[issues-badge]: https://img.shields.io/github/issues/azure/device-telemetry-dotnet.svg
[issues-url]: https://github.com/azure/device-telemetry-dotnet/issues
[gitter-badge]: https://img.shields.io/gitter/room/azure/iot-solutions.js.svg
[gitter-url]: https://gitter.im/azure/iot-solutions

[project-wiki]: https://github.com/Azure/device-telemetry-dotnet/wiki/%5BAPI-Specifications%5D-Messages
[documentdb-url]: https://docs.microsoft.com/en-us/azure/cosmos-db/create-documentdb-dotnet
[storageadapter-url]: https://github.com/Azure/pcs-storage-adapter-dotnet/blob/master/README.md
[azurestorageconnstring-url]: https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string#create-a-connection-string-for-an-azure-storage-account
[postman-url]: https://www.getpostman.com
[vs-install-url]: https://www.visualstudio.com/downloads
[dotnetcore-tools-url]: https://www.microsoft.com/net/core#windowsvs2017
[windows-envvars-howto-url]: https://superuser.com/questions/949560/how-do-i-set-system-environment-variables-in-windows-10
[docker-compose-install-url]: https://docs.docker.com/compose/install
