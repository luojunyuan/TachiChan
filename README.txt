ErogeHelper                   net7-WinRT    net8 native                  net8 WinRT native
ErogeHelper.AssistiveTouch    net7          net8 R2R wpf                 net8 WinRT R2R wpf
Preference                    uwp           uwp                          uwp (min 1809 17763)
ErogeHelper.TextWindow                      net8 native winforms
ErogeHelper.KeyMapping                      c++ exe
ErogeHelper.VirtualKeyboard                 net8 native winforms

-windows10.0.19041.0 enable WinRT

publish
Release -> Package -> "Publish Application"

debug
Debug -> x64 -> Package -> "Run"

never think back to netfx

ストア版ですから、Erogeの単語を想定しないほうがいいかも

---
Issue with Windows.ApplicationModel.AppService
https://github.com/microsoft/CsWinRT/issues/1172
可行性验证, 目前使用AppServer, Notification
1. PureCSWinRT
无法凑齐AppService编译所需引用，无法生成enum，见上述issue
2. C++/WinRT component
不用native正常运行，自建类型会报type convert COMException
不会写c++/WinRT，AppService一个毫毛都写不出来
backup
C++ project from regFree_WinRT
CSharp net8 reference CSWinRT, VCRTruntime, tag...
3. 尝试 c++winrt exe, 从uwp启动两个win32程序
---
