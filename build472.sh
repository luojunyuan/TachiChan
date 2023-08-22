#!/bin/bash
xml_files=("TouchChan/TouchChan.csproj" 
    "TouchChan.SplashScreen/TouchChan.SplashScreen.csproj"
    "TouchChan.AssistiveTouch/TouchChan.AssistiveTouch.csproj"
    "TouchChan.AssistiveTouch.Gesture/TouchChan.AssistiveTouch.Gesture.csproj"
)
for xml_file in "${xml_files[@]}"; do
    sed -i 's|<TargetFramework>.*</TargetFramework>|<TargetFramework>net472</TargetFramework>|' "$xml_file"
done
dotnet build -c Release -o bin/TachiChan TouchChan/TouchChan.csproj
dotnet build -c Release -o bin/TachiChan TouchChan.AssistiveTouch/TouchChan.AssistiveTouch.csproj
dotnet build -c Release -o bin/TachiChan TouchChan.AssistiveTouch.Gesture/TouchChan.AssistiveTouch.Gesture.csproj
dotnet build -c Release -o bin/TachiChan Preference.Win32/Preference.Win32.csproj
find bin/TachiChan -type f \( -name "*.pdb" -o -name "*.exe.config" \) -exec rm -f {} \;
tar -czf net472.tar.gz -C bin/TachiChan .