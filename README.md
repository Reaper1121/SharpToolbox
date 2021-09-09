﻿![Discord Shield](https://discordapp.com/api/guilds/489390392970838016/widget.png?style=shield)
# SharpToolbox
This is a 64-Bit cross-platform library supporting .NET 5 and higher that contains various utitilies and other useful things to help out the development of projects.

## Current state:

This library is currently in super early state and requires a lot of work and will be sperated into multiple NuGet packages once its big enough, the name of this project may not be final and will be renamed to something more approriate.

## Build Requirements

 1. Visual Studio 2022/2019 Preview (Optional)
 2. .NET 5 SDK

## Build Instructions (Visual Studio): 

 1. From Visual Studio 2022 Installer, make sure ".NET Desktop Environment" is installed.
 2. When the project is open, select the build configuration at the top (Debug/Release).
 3. Right click on the project in the solution explorer and click "Build".
 4. Use the "Publish" instead of "Build" for more options. (Optional)

## Build Instructions (Terminal):

 1. Open your terminal at the project directory.
 2. Run the command: dotnet publish -c Release -a x64