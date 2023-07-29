Code Name: TouchChan
表紙ネーム：TachiChan - タッチちゃん - 触控酱(触摸酱TapChan？どっち)

### build
* Universal Windows Platform Devolopment (about 12gb)
Preference (UWP)
TouchChanPackage
* C++ Desktop Devolopment (MSVC v14* C++ x86/x64/arm64 build tools about 3gb)
WinRTLauncher
--- Finally can publish the package

### debug
Debug -> x64 -> Package -> "Run"

### publish
Release -> Package -> "Publish Application"

##### Find My Future
1. 为 shinario 启动旧式touch（测试可行，焦点都在touch上）（预计工作量至少一天，若有人提及再优先做）
2. 消除对 WindowsInput 的依赖

backup
TouchChan.Preference

.editorconfig changes
1.DllImport

ErogeHelper                   net7-WinRT    net8 native                  net8 WinRT native
ErogeHelper.AssistiveTouch    net7          net8 R2R wpf                 net8 WinRT R2R wpf
Preference                    uwp           uwp                          uwp (min 1809 17763)
ErogeHelper.TextWindow                      net8 native winforms
ErogeHelper.VirtualKeyboard                 net8 native winforms
-windows10.0.19041.0 enable WinRT

记录一下我用到了哪些winrt服务，因为不想污染native的TouchChan，所以main与winrt无关。
（c++/winrt 启动器中）
1.Appservice与UWP交互
（TouchChan.AssistiveTouch中）
2.Toast用来提示是否是管理员模式
3.Battery模块用来检查设备是否带电池（考虑将来替换原生粗糙的实现）

--- winrt native可行性验证，目前使用的AppServer, Notification
* 决定的方向，用c++winrt实现
    a 通过选择进程启动，pass一个参数 --channel，筛选进程，获取图标bitmap，json发送
    b 输入 --path 参数，后接游戏路径。在c++winrt中读取TouchChan需要的配置信息。通过命令行传输这些选项。
      在此启动上一层级的TouchChan程式
    c ExTip，ShellHandle启动的是这个WinRTLauncher程序
---
