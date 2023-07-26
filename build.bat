REM this file is for testing temporary using. Aim at net7 arm64
set dest=../Release/TachiChan
dotnet build TouchChan/TouchChan.csproj -c Release -r win-arm64 -o %dest%/TouchChan
dotnet build TouchChan.AssistiveTouch/TouchChan.AssistiveTouch.csproj -c Release -r win-arm64 -o %dest%/TouchChan.AssistiveTouch
dotnet build TouchChan.AssistiveTouch.Gesture/TouchChan.AssistiveTouch.Gesture.csproj -c Release -r win-arm64 -o %dest%/TouchChan.AssistiveTouch.Gesture --no-self-contained
powershell -Command "& {ls ../Release/TachiChan -include *.pdb -recurse | rm}"
powershell -Command "& {ls ../Release/TachiChan -include *.exe.config -recurse | rm}"