@ECHO off & setlocal enableextensions enabledelayedexpansion

:: Note: use lowercase names for the Docker images
SET DOCKER_IMAGE="azureiotpcs/telemetry-dotnet"

:: strlen("\scripts\docker\") => 16
SET APP_HOME=%~dp0
SET APP_HOME=%APP_HOME:~0,-16%
cd %APP_HOME%

:: The version is stored in a file, to avoid hardcoding it in multiple places
set /P APP_VERSION=<%APP_HOME%/version

:: Whether to update the "latest" tag of the Docker image
    SET BUILD_LATEST="no"
    echo Building version %APP_VERSION% - %DOCKER_IMAGE%:%APP_VERSION%
    SET /p RESPONSE="Do you want to publish also the 'latest' version? [y/N] "
    IF "%RESPONSE%" == "y" (SET BUILD_LATEST="yes")
    IF "%RESPONSE%" == "Y" (SET BUILD_LATEST="yes")

:: Publish to Docker hub
    if %BUILD_LATEST% == "no" (
        docker push %DOCKER_IMAGE%:%APP_VERSION%
    ) else (
        docker push %DOCKER_IMAGE%:%APP_VERSION%
        docker push %DOCKER_IMAGE%:latest
    )

endlocal
