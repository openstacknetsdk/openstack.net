# openstack.net: An OpenStack SDK for Microsoft .NET

[![Join the chat at https://gitter.im/openstacknetsdk/openstack.net](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/openstacknetsdk/openstack.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

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
[CONTRIBUTING](CONTRIBUTING.md).

## Building from Source

### Prerequisites

The recommended development environment for this project is Visual Studio 2013 or Visual Studio 2015. When using Visual
Studio 2015, a [custom analyzer](https://github.com/openstacknetsdk/OpenStackNetAnalyzers) is provided
which uses static analysis to identify common mistakes, and in many cases is able to automatically fix the issue.

### Build script

Execute `build.cmd` to download all dependencies and build. Use `build.cmd help` or `build.cmd /?` to view the available command line arguments.

```bash
build.cmd [Build|UnitTest|Documentation|Package] [/Configuration Debug|Release]

# Execute Build target in Debug mode
build.cmd

# Execute UnitTest target in Debug mode
build.cmd UnitTest

# Execute Build target in Release mode
build.cmd /Configuration Release

# Execute Package target in Release mode
build.cmd Package /Configuration Release
```

See the [Documentation README](src/Documentation/README.md) if you would like to build the Sandcastle documentation which is published to http://docs.openstacknetsdk.org.

### Integration Tests
You must have an OpenStack environment to test against in order to run the integration tests. We use the [all-in-one OpenStack Ansible Deployment](https://github.com/stackforge/os-ansible-deployment/blob/master/development-stack.rst) to set everything up on a single server. The tests look for the identity endpoint and credentials in environment variables: OPENSTACKNET_USER, OPENSTACKNET_PASSWORD, OPENSTACKNET_PROJECT and OPENSTACKNET_IDENTITY_URL. After you have set the environment variables you will need to log out then log back in.

```batchfile
setx OPENSTACKNET_USER secretusername
setx OPENSTACKNET_PASSWORD secretpassword
setx OPENSTACKNET_PROJECT projectname
setx OPENSTACKNET_IDENTITY_URL http://localhost:5000/v2.0
```

#### This is not an official OpenStack project
