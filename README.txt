2023年6月-2024年5月

Code Name: TouchChan
表紙ネーム：TachiChan - タッチちゃん - 触控酱

一款为美少女游戏触控生态而生的辅助软件，专为平板电脑打造。

### build (Visual Studio Installer)

你可以使用 dotnet-cli 来编译支援到 win7 的 tachichan，只需运行 build472.bat 即可生成到 bin/ 目录下

Workload

* .Net Desktop Devolopment
* C++ Desktop Devolopment
* Universal Windows Platform Desktop

Additional

* Windows 10 SDK (10.0.18362.0)                    // for compatibal to win10 1903
* MSVC v14* C++ Arm64/Arm64EC build tools (latest) // for WinRTLauncher and .net native compile 

Minimal compile

If you only wanna build net472, you can use dotnet cli, run build472.bat. Or install .Net Desktop Devolopment.

### debug (recommand build >= 22000 to use select process to start)
1. Set AssistiveTouch OutputType->Exe, Console.ReadKey() to pause. Try attach to `TouchChan.AsstiveTouch.exe` in VS
2. Debug -> x64 -> Package -> Run

### publish
Cofiguration Manager: Release -> Project: Package -> Right click on package project: "Publish Application"

> 使用多进程结构的理由

1. 因为WPF使用了DWM管控的TransparentWindow，实在没法控制内存爆增过高，因无法忍受内存大小故，使用多进程有效缓解这个现状，因为让wpf更独立性。如果不使用wpf在考虑单TOUCH进程
2. 仅在AssistiveTouch启动时读入用户配置，Preference对配置进行编辑。（AssistiveTouch结束时存储按钮位置是个例外）
3. Child window的结构会让touch随着游戏窗口的销毁而销毁，部分游戏存在启动器销毁，全屏切换销毁window handle的情景。所以需要常驻一个main进程（TouchChan）来重启AssistiveTouch。因为game handle改变了，必须重新查找handle。这部分逻辑也是依赖入口点进程，AssistiveTouch中依赖的game main handle为只读。
4. Gesture lifetime with AssistiveTouch. 模块化，保证功能独立性故与wpf项目分离，AssistiveTouch.Core下的功能已经够多了。自己实现鼠标键盘钩子没有回调的必要？
5. Virtual Keyboard lifetime 由 AssistiveTouch 控制 确实没有回调Touch模拟键盘按下的必要
6. GameController lifetime with AssistiveTouch 所有功能独立，仅接入断开由AssistiveTouch提示。(不要SharpDX太重了) 也许需要XInput1_4.dll 所以不要支持win7

> TachiChan使用到的winrt服务，旨在简化开发，不再兼容，抛弃win8前旧平台。（考虑Store应用会导致命令行更难与第三方应用交互）

（c++/winrt Launcher）

1. Appservice与UWP交互

（TouchChan.AssistiveTouch）

2. Toast用来提示是否是管理员模式
3. Battery模块用来检查设备是否带电池（考虑将来替换原生实现）
4. StorageFolder搭配UWP获取程序配置路径

FIXME
1. tantei7 不论 SetWindowsPos 如何设置宽高，只会保持原始大小，但是BorderlessGaming是正常的
2. krkr 的游戏普通触控会顶替触控手势的模拟，并且触控没有办法屏蔽，应该是对触摸自带处理支持
4. fix some warnings of handle
5. 游戏分辨率切换或系统分辨率、dpi切换导致的大小不合适问题
