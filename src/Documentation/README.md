The documentation uses a [special build of Sandcastle Help File Builder (SHFB)](https://github.com/tunnelvisionlabs/SHFB/releases/tag/v2014.11.22.0-beta).

To build the documentation from the command line, no install is required. Just run the following:

    nuget restore src\\Documentation\\Documentation.sln
    msbuild src\\Documentation\\Documentation.sln

If you want to edit the documentation using Visual Studio, you must first install the [custom build of Sandcastle Help File Builder](https://github.com/tunnelvisionlabs/SHFB/releases/download/v2014.11.22.0-beta/SandcastleHelpFileBuilder.msi) and then install the [custom Sandcastle Help File Builder Visual Studio extension](https://github.com/tunnelvisionlabs/SHFB/releases/download/v2014.11.22.0-beta/SHFBVisualStudioPackage.vsix).
