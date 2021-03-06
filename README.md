dnvm
----

The .NET Core version manager.
The easiest way to install and manage versions of .NET Core tools, runtimes, and SDKs.

## Getting started

**Install**
```
brew tap natemcmaster/dnvm
brew install dnvm
```

**Use**
```
dnvm install sdk
mkdir MyFirstApp
dotnet new console
dotnet restore
dotnet run
```

## Usage

Install .NET Core SDK

```sh
# install the most recent, stable .NET Core SDK.
# these commands are equivalent
dnvm install sdk
dnvm install sdk stable

# install a specific version
dnvm install sdk 1.0.0-rc4-004915
```

Install a .NET Core runtime

```sh
# install the most recent stable .NET Core runtime
# these commands are equivalent
dnvm install runtime
dnvm install runtime stable

# install a specific version of .NET Core runtime
dnvm install runtime 1.1.0
```

Install a .NET Core CLI tool

```sh
# install the most recent stable .NET Core CLI tool
# these commands are equivalent
dnvm install tool watch
dnvm install tool watch stable

# install a specific version
dnvm install tool watch 1.0.0

# using this tool after it is installed
# these tools are installed globally, not just to a specific project
dotnet watch
```

List versions that could be installed
```sh
# list all known versions of .NET Core runtime
dnvm list runtime
# list all known versions of .NET Core SDK
dnvm list sdk
```

Show what is installed
```sh
dnvm info
```

Remove stuff
```sh
# uninstalls the .NET Core SDK
dnvm rm sdk 1.0.0-rc4-004915
# uninstalls the .NET Core runtime
dnvm rm runtime 1.0.0
```

## The dnvm config file (optional)

A file named `.dnvm` can be placed in a project to configure
the versions of .NET Core runtimes, SDKs, and tools that should be installed.

Example:

```yaml
# required: specify an environment name
envName: default
# optional: list the .NET Core SDK to install
sdk: 1.0.0-rc4-004915
# optional: list versions of .NET Core to install
runtimes:
  - 1.0.1
  - 1.1.0
  - stable
# optional: list of .NET Core CLI tools to install
tools:
  watch: stable
```

It will define the 'environment' for all `dotnet` and `dnvm` commands
executed in the directory containing the file, or any subdirectories.

Executing `dnvm install` will install all items in the effective `.dnv`m` file.

### Creating the file

```sh
# creates a .dnvm file in the current folder
dnvm init
```

### Modifying the file
```sh
# installs .NET Core SDK and saves the version to the .dnvm file
dnvm install sdk --save

# installs .NET Core runtime and saves the version to the .dnvm file
dnvm install runtime 1.1.0 --save
```
