# Task 2.1.1 - Add Centennial Support using Visual Studio 2017

This task will guide you through the process of converting a Win32 Desktop app to a UWP App using Visual Studio 2017. 

## Prerequisites 

* Basic knowledge of C# development
* Basic knowledge of client development with the .NET framework
* Basic knowledge of Windows 10 and the Universal Windows Platform
* A computer with Windows 10 Anniversary Update or Windows 10 Creators Update. If you want to use the Desktop App Converter with an installer, you will need at least a Pro or Enterprise version, since it leverages a feature called Containers which isn’t available in the Home version.
* Visual Studio 2017 with the tools to develop applications for the Universal Windows Platform. Any edition is supported, including the free [Visual Studio 2017 Community](https://www.visualstudio.com/vs/community/)

## Task: Add Centennial support in Visual Studio

#### Step 1: Open your existing Win32 Solution with Visual Studio 2017

If you don't have an existing project, create a new C# Windows Classic Desktop Windows Forms App or WPF App project. Name the project MyDesktopApp.

![New C# Project](images/211-new-project.png)

#### Step 2: Add a UWP Project

To create a Desktop Bridge package, add a JavaScript UWP project to the your solution.

Note: even though we are using a JavaScript UWP template, we are not going to write any JavaScript code. We are only using the project as a tool
to convert your Win32 app to a UWP app.

Right-click on the Solution folder and select **Add | New Project...**

![Add Project](images/211-add-project.png)

Select the **JavaScript | Windows Universal | Blank App (Universal Windows)** project template. Name the project MyDesktopApp.Package.

![Add JS UWP Project](images/211-add-js-uwp.png)

Make sure the minimum Windows SDK version is **14393** or higher. Windows 10 14393 or higher is required for Desktop Bridge apps.

![Minimum SDK version](images/211-sdk-version.png)

You solution should now contain 2 projects.

![Solution projects](images/211-solution.png)

In order for the UWP app to pick up any changes you make to the Win32 app, you should add a Project Build Dependency to the UWP app so it depends on the Win32 app.
Right-click on the MyDesktopApp.Package project and select **Build Dependencies | Project Dependencies...** from the menu.

![Build dependencies](images/211-build-dependencies.png)

Select **MyDesktopApp** and click **OK**

![Build dependencies](images/211-win32-dependency.png)

Press F7 or select Build Solution from the Visual Studio Build menu. The Win32 and UWP projects will build and the MyDesktopApp Win32 binaries will be copied to the 
win32 folder of the MyDesktopApp.Package project.


#### Step 3: Add the Win32 binaries to the UWP Project

In order to convert your Win32 app to a Desktop Bridge UWP app, you will need to add the binaries created by your app to the JavaScript UWP app. 
We are going to use the JavaScript UWP app to create the app package that will eventually be submitted to the Windows Store.

All the Win32 binaries will be stored in your UWP project in a folder called win32 (though this exact name is not required; you can use any name you like).
You can automate the project to copy the files after each build, improving your development workflow. Edit your project file (MyDesktopApp.csproj in this example) 
to include an AfterBuild target that will copy all the Win32 output files to the win32 folder in the UWP project as follows:

```xml
  <Target Name="AfterBuild">
    <PropertyGroup>
      <TargetUWP>..\MyDesktopApp.Package\win32\</TargetUWP>
    </PropertyGroup>
     <ItemGroup>
       <Win32Binaries Include="$(TargetDir)\*" />
     </ItemGroup>
    <Copy SourceFiles="@(Win32Binaries)" DestinationFolder="$(TargetUWP)" />
  </Target>
```

You can edit the MyDesktopApp.csproj in Visual Studio by right clicking on the project and selecting **Unload project**

![Unload project](images/211-unload-project.png)

Right-click again on MyDesktopApp.csproj and select **Edit MyDesktopApp.csproj**

![Edit project](images/211-edit-project.png)

Scroll to the end of the xml file and paste the above xml code to the end of the file before the final project tag.

```xml
  <Target Name="AfterBuild">
    <PropertyGroup>
      <TargetUWP>..\MyDesktopApp.Package\win32\</TargetUWP>
    </PropertyGroup>
    <ItemGroup>
      <Win32Binaries Include="$(TargetDir)\*" />
    </ItemGroup>
    <Copy SourceFiles="@(Win32Binaries)" DestinationFolder="$(TargetUWP)" />
  </Target>
</Project>
```

Save your changes and then reload MyDesktopApp.csproj.

![Reload project](images/211-reload-project.png)

Now that the Win32 binaries are copied to the UWP project after a build, we need to add the binaries to the UWP project so they will be packaged with the UWP app.
We can automate this process by editing the MyDesktopApp.Package.jsproj project. Following the same procedure you just completed when editing the MyDesktopApp.csproj file, 
you will unload, edit and reload the MyDesktopApp.Package.jsproj project file. Add the following XML to the end of the MyDesktopApp.Package.jsproj project file.

```xml
  <ItemGroup>
    <Content Include="win32\*.dll">
      <Link>win32\%(RecursiveDir)%(FileName)%(Extension)</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="win32\*.exe">
      <Link>win32\%(RecursiveDir)%(FileName)%(Extension)</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="win32\*.config">
      <Link>win32\%(RecursiveDir)%(FileName)%(Extension)</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="win32\*.pdb">
      <Link>win32\%(RecursiveDir)%(FileName)%(Extension)</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
```

After you reload the MyDesktopApp.Package.jsproj project file, your project should now contain the Win32 binaries from your desktop app.

![Win32 Binaries](images/211-uwp-win32-binaries.png)

Right click on the MyDesktopApp.exe file and select **Properties**. In the Properties window notice that the file is marked Content and Copy if newer is enabled.
These settings will ensure that your Win32 binaries will become part of the UWP app's AppX package.

![Content](images/211-content.png)

#### Step 4: Edit the App Manifest to enable the Desktop Bridge Extensions

The MyDesktopApp.Package project contains a file called package.appxmanifest that describes how to package your app for the Windows Store and its dependencies.
We need to edit this file so it includes the infomation needed to run our Win32 app as a UWP app. 

You can edit the package.appxmanifest xml file by right-clicking on the file in the Visual Studio project and selecting **View Code**

![View Code](images/211-view-code.png)

Replace the line (near line 6):

```xml
    IgnorableNamespaces="uap mp">
```

with

```xml
    xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities" IgnorableNamespaces="uap rescap">
```

Replace the line (near line 22):

```xml
    <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.0.0" MaxVersionTested="10.0.0.0" />
```

with

```xml
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.14393.0" MaxVersionTested="10.0.14393.0" />  
```

Add the following capability to the Capabilities section (near line 48)

```xml
    <rescap:Capability Name="runFullTrust" />
```

Modify the Application tag to the following (near line 30):

```xml
    <Application Id="MyDesktopApp" Executable="win32\MyDesktopApp.exe" EntryPoint="Windows.FullTrustApplication">
```

Change the DisplayName near line 33 from "MyDesktopApp.Package" to "MyDesktopApp"


Your package.appxmanifest should now look something like:

```xml
    <?xml version="1.0" encoding="utf-8"?>
    <Package
      xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
      xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
      xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
      xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities" IgnorableNamespaces="uap rescap">

      <Identity
        Name="c0063394-4c57-415d-89b5-8d1b5fcbbdf8"
        Version="1.0.0.0"
        Publisher="CN=dalestam" />

      <mp:PhoneIdentity PhoneProductId="c0063394-4c57-415d-89b5-8d1b5fcbbdf8" PhonePublisherId="00000000-0000-0000-0000-000000000000" />

      <Properties>
        <DisplayName>MyDesktopApp</DisplayName>
        <PublisherDisplayName>dalestam</PublisherDisplayName>
        <Logo>images\storelogo.png</Logo>
      </Properties>

      <Dependencies>
        <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.14393.0" MaxVersionTested="10.0.14393.0" />
      </Dependencies>

      <Resources>
        <Resource Language="x-generate" />
      </Resources>

      <Applications>
        <Application Id="MyDesktopApp" Executable="win32\MyDesktopApp.exe" EntryPoint="Windows.FullTrustApplication">

          <uap:VisualElements
            DisplayName="MyDesktopApp.Package"
            Description="MyDesktopApp.Package"
            BackgroundColor="transparent"
            Square150x150Logo="images\Square150x150Logo.png"
            Square44x44Logo="images\Square44x44Logo.png">

            <uap:DefaultTile Wide310x150Logo="images\Wide310x150Logo.png" />
            <uap:SplashScreen Image="images\splashscreen.png" />

          </uap:VisualElements>
        </Application>
      </Applications>

      <Capabilities>
        <Capability Name="internetClient" />
        <rescap:Capability Name="runFullTrust" />
      </Capabilities>

    </Package>
```
Note: Your Publisher, PublisherDisplayName and other app id properties will be different.

#### Step 5: Deploy and run your converted Win32 App

Your converted Win32 app is now ready to be deployed and run on your computer.

Select **Deploy** from the Visual Studio Build menu.

![Deploy](images/211-deploy.png)

Search for your MyDesktopApp in the Start menu. Click on MyDesktopApp to launch your app. 

![Start menu](images/211-startmenu.png)

Your Win32 app has now been converted to a UWP App using Visual Studio.

#### Step 6: Debugging your Desktop Bridge app

In order to debug your UWP app, you need to set the UWP JavaScript MyDesktopApp.Package project as the startup project. 
Right-click on the MyDesktopApp.Package project and select **Set as StartUp Project**

![Startup Project](images/211-startup-project.png)

Press F5 to start your UWP app. You will probably encounter the following error:

![Debug Error](images/211-debug-error.png)

We will fix this error in the [next task](212_Debugging.md).


## References

* https://docs.microsoft.com/en-us/windows/uwp/porting/desktop-to-uwp-packaging-dot-net 
* https://github.com/qmatteoq/BridgeTour-Workshop
* https://mva.microsoft.com/en-us/training-courses/developers-guide-to-the-desktop-bridge-17373