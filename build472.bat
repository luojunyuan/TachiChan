set csprojPath=".\TouchChan\TouchChan.csproj"
PowerShell.exe -ExecutionPolicy Bypass -File "WriteFramework.ps1" -csprojPath %csprojPath%
set csprojPath=".\TouchChan.AssistiveTouch\TouchChan.AssistiveTouch.csproj"
PowerShell.exe -ExecutionPolicy Bypass -File "WriteFramework.ps1" -csprojPath %csprojPath%
set csprojPath=".\TouchChan.AssistiveTouch.Gesture\TouchChan.AssistiveTouch.Gesture.csproj"
PowerShell.exe -ExecutionPolicy Bypass -File "WriteFramework.ps1" -csprojPath %csprojPath%
set csprojPath=".\TouchChan.AssistiveTouch.Gamepad\TouchChan.AssistiveTouch.Gamepad.csproj"
PowerShell.exe -ExecutionPolicy Bypass -File "WriteFramework.ps1" -csprojPath %csprojPath%
set csprojPath=".\TouchChan.AssistiveTouch.VirtualKeyboard\TouchChan.AssistiveTouch.VirtualKeyboard.csproj"
PowerShell.exe -ExecutionPolicy Bypass -File "WriteFramework.ps1" -csprojPath %csprojPath%
set csprojPath=".\TouchChan.SplashScreen\TouchChan.SplashScreen.csproj"
PowerShell.exe -ExecutionPolicy Bypass -File "WriteFramework.ps1" -csprojPath %csprojPath%
set dest=bin/TachiChan
dotnet build -c Release -o %dest% TouchChan/TouchChan.csproj
dotnet build -c Release -o %dest% TouchChan.AssistiveTouch/TouchChan.AssistiveTouch.csproj
dotnet build -c Release -o %dest% TouchChan.AssistiveTouch.Gamepad/TouchChan.AssistiveTouch.Gamepad.csproj
dotnet build -c Release -o %dest% TouchChan.AssistiveTouch.Gesture/TouchChan.AssistiveTouch.Gesture.csproj
dotnet build -c Release -o %dest% TouchChan.AssistiveTouch.VirtualKeyboard/TouchChan.AssistiveTouch.VirtualKeyboard.csproj
dotnet build -c Release -o %dest% Preference.Win32/Preference.Win32.csproj
powershell -Command "& {ls bin/TachiChan -include *.pdb -recurse | rm}"
powershell -Command "& {ls bin/TachiChan -include *.exe.config -recurse | rm}"