Code Name: TouchChan
表紙ネーム：TachiChan - タッチちゃん - 触控酱

肯定优先商店publish，再清理csproj
TouchChan.Preference

1. 用最新的编程方法来编写
2. 最少的发行属性，完善的一键发行
3. 向前兼容到win7
4. 无任何外部依赖，纯原生实现
5. win11 mica 以下 透明模糊
6. package支持到 17763 1809，不知可否还能再早可能需要另外装sdk
7. 还是想要支持win7但是，一种是net8的native，一种是netfx，看到

关掉的提示 
DllImport

向前兼容，非商店版本向前兼容到win7，达到仅在修改三个项目TFM为netfx472的前提下就能成功编译运行，增加Preference.Win32辅助
代码里的winrt/net8不兼容的部分，使用fx472格式，宏定义条件的方式增加代码。不作为优先项目实现。

移除WindowsInput
移除LnkShotcut
新建TouchChan.SplashScreen,使用宏，合并为一个，比如dllimport。

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
