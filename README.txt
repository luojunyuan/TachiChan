Code Name: TouchChan
表紙ネーム：TachiChan - タッチちゃんー触控酱

ErogeHelper                   net7-WinRT    net8 native                  net8 WinRT native
ErogeHelper.AssistiveTouch    net7          net8 R2R wpf                 net8 WinRT R2R wpf
Preference                    uwp           uwp                          uwp (min 1809 17763)
ErogeHelper.TextWindow                      net8 native winforms
ErogeHelper.KeyMapping                      c++ exe
ErogeHelper.VirtualKeyboard                 net8 native winforms

-windows10.0.19041.0 enable WinRT

记录一下我用到了哪些winrt服务，因为不想污染native的TouchChan，所以main与winrt无关。
（c++/winrt 启动器中）
1.Appservice与UWP交互
（TouchChan.AssistiveTouch中）
2.Toast用来提示是否是管理员模式
3.Battery模块用来检查设备是否带电池（考虑将来替换原生粗糙的实现）

publish
Release -> Package -> "Publish Application"

debug
Debug -> x64 -> Package -> "Run"

never think back to netfx

ストア版ですから、Erogeの単語を想定しないほうがいいかも

--- winrt native可行性验证，目前使用的AppServer, Notification
1. PureCSWinRT
无法凑齐AppService编译所需引用，无法生成enum，见issue
Issue with Windows.ApplicationModel.AppService
https://github.com/microsoft/CsWinRT/issues/1172
2. C++/WinRT component
不用native正常运行，自建类型会报type convert COMException
不会写c++/WinRT，AppService一个毫毛都写不出来
backup
C++ project from regFree_WinRT
CSharp net8 reference CSWinRT, VCRTruntime, tag...
Edit2: 有很多进展，也是必须warpper一个c#winrtLib，但是目前有不明原因的IOException报错
3. 尝试 c++winrt exe, 从uwp启动两个win32程序
4. 决定的方向，用c++winrt实现
    a 通过选择进程启动，pass一个参数 --channel，筛选进程，获取图标bitmap，json发送
    b 输入 --path 参数，后接游戏路径。在c++winrt中读取TouchChan需要的配置信息。通过命令行传输这些选项。
      在此启动上一层级的TouchChan程式
    c ExTip，ShellHandle启动的是这个WinRTLauncher程序
---
