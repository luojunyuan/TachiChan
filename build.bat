set dest=../Publish/TachiChan
dotnet build TouchChan/TouchChan.csproj -c Release -r win-arm64 -o %dest%/TouchChan
dotnet build TouchChan.AssistiveTouch/TouchChan.AssistiveTouch.csproj -c Release -r win-arm64 -o %dest%/TouchChan.AssistiveTouch
dotnet build TouchChan.AssistiveTouch.Gesture/TouchChan.AssistiveTouch.Gesture.csproj -c Release -r win-arm64 -o %dest%/TouchChan.AssistiveTouch.Gesture --no-self-contained
powershell -Command "& {ls ../Publish/TachiChan -include *.pdb -recurse | rm}"
powershell -Command "& {ls ../Publish/TachiChan -include *.exe.config -recurse | rm}"