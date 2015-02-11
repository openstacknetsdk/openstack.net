# openstack.net: An OpenStack SDK for Microsoft .NET

The openstack.net SDK, written for the Microsoft .NET platform, is designed to enable developers to seamlessly work with
the many services provided by the OpenStack cloud operating system.

The openstack.net SDK contains:

* A language API
* Getting Started Guide
* API Reference Manual
* Release Notes
* Sample code

## Contributing

We welcome and encourage contributions from the developer community. For an overview of the contribution process,
including an explanation of our issue labels and common emoji used in discussions, please see
[CONTRIBUTING.md](CONTRIBUTING.md).

## Building from Source

### Prerequisites

The recommended development environment for this project is Visual Studio 2013 or Visual Studio 2015. When using Visual
Studio 2015, a [custom analyzer][OpenStackNetAnalyzers] is automatically provided
which uses static analysis to identify common mistakes, and in many cases is able to automatically fix the issue.

To edit the documentation projects within Visual Studio, Sandcastle Help File Builder (SHFB) must be installed on the
development machine. Currently the project uses a [custom build of SHFB][SHFB]. Automated builds do not require manual
installation of SHFB, as described below.

[OpenStackNetAnalyzers]: https://github.com/openstacknetsdk/OpenStackNetAnalyzers
[SHFB]: https://github.com/tunnelvisionlabs/SHFB/releases/tag/v2014.11.22.0-beta

### Build script

The complete project may be built from source using the **build.ps1** PowerShell script. In addition to the library
binaries, this script will create the NuGet packages for the SDK. The `-InstallSHFB` command ensures that the correct
version of SHFB is used for the automated build (and uses NuGet to download it on demand).

```powershell
cd build
.\build.ps1 -InstallSHFB
```

To build the project without documentation, use the `-NoDocs` switch. This switch is equivalent to using the
**ReleaseNoDocs** solution configuration instead of the normal **Release** configuration.

```powershell
cd build
.\build.ps1 -NoDocs
```

#### This is not an official OpenStack project
