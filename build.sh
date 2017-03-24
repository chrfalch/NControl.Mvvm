#!/bin/bash
#
set -e # Exit on any failure for any command
set -x # Print all commands before running them - for easy debugging.

## Clean
xbuild /p:Configuration=Release /p:Platform=Release /target:Clean /p:OutputPath=build NControl.MVVM/NControl.Mvvm.csproj
rm -rf NControl.MVVM/build/
xbuild /p:Configuration=Release /p:Platform=Release /target:Clean /p:OutputPath=build NControl.MVVM.iOS/NControl.Mvvm.iOS.csproj
rm -rf NControl.MVVM.iOS/build/
xbuild /p:Configuration=Release /p:Platform=Release /target:Clean /p:OutputPath=build NControl.MVVM.Droid/NControl.Mvvm.Droid.csproj
rm -rf NControl.MVVM.Droid/build/

xbuild /p:Configuration=Release /p:Platform=Release /target:Clean /p:OutputPath=build NControl.MVVM.Fluid/NControl.Mvvm.Fluid.csproj
rm -rf NControl.MVVM.Fluid/build/
xbuild /p:Configuration=Release /p:Platform=Release /target:Clean /p:OutputPath=build NControl.MVVM.Fluid.iOS/NControl.Mvvm.Fluid.iOS.csproj
rm -rf NControl.MVVM.Fluid.iOS/build/
xbuild /p:Configuration=Release /p:Platform=Release /target:Clean /p:OutputPath=build NControl.MVVM.Fluid.Droid/NControl.Mvvm.Fluid.Droid.csproj
rm -rf NControl.MVVM.Fluid.Droid/build/

## Build with parameters
xbuild /p:Configuration=Release /p:Platform=Release /target:build /p:OutputPath=build NControl.MVVM/NControl.Mvvm.csproj
xbuild /p:Configuration=Release /p:Platform=Release /target:build /p:OutputPath=build NControl.MVVM.iOS/NControl.Mvvm.iOS.csproj
xbuild /p:Configuration=Release /p:Platform=Release /target:build /p:OutputPath=build NControl.MVVM.Droid/NControl.Mvvm.Droid.csproj

xbuild /p:Configuration=Release /p:Platform=Release /target:build /p:OutputPath=build NControl.MVVM.Fluid/NControl.Mvvm.Fluid.csproj
xbuild /p:Configuration=Release /p:Platform=Release /target:build /p:OutputPath=build NControl.MVVM.Fluid.iOS/NControl.Mvvm.Fluid.iOS.csproj
xbuild /p:Configuration=Release /p:Platform=Release /target:build /p:OutputPath=build NControl.MVVM.Fluid.Droid/NControl.Mvvm.Fluid.Droid.csproj

mkdir -p ../build
nuget pack -OutputDirectory ../build NControl.MVVM.Fluid.nuspec 
nuget pack -OutputDirectory ../build NControl.MVVM.nuspec 
nuget pack -OutputDirectory ../build Animation/NControl.XAnimation.nuspec 
