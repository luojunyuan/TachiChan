Code Name: TouchChan
表紙ネーム：TachiChan - タッチちゃん - 触控酱

———— TachiChan，一个为Galgame触控生态而生的辅助软件，专为平板电脑，串流，掌机设备以及手柄操控而打造。

### build (Visual Studio Installer)
Workload
* .Net Desktop Devolopment
* C++ Desktop Devolopment
* Universal Windows Platform Desktop
Additional
* Windows 10 SDK (10.0.18362.0) // for compatibal to win10 1903
* MSVC v14* C++ Arm64/Arm64EC build tools (latest) // for WinRTLauncher and .net native compile 

Minimal compile
If you only wanna build net472, you can use dotnet cli, run build472.bat. Or install .Net Desktop Devolopment.
If you need to test UWP and net8 vertion. Select
* .Net Desktop Devolopment (inherit `.NET Profile tool` `Blend for Visual Studio` `MSIX Packaging Tools`) (2.23GB)
* C++ Desktop Devolopment (inherit `Windows 11 SDK (10.0.22xxx.0)`)
  - MSVC v14* C++ x64/x86 build tools (latest) (2.2GB)
* Universal Windows Platform Desktop (11.05GB)
Then you can compile hole project to do test. (total 15.48GB)

### debug
Debug -> x64 -> Package -> "Run"

### publish
Release -> Package -> "Publish Application"

##### Find My Future
More test on different devices
首次运行引导（不使用游戏进行说明，使用自己的截图或者动图全屏进行说明）(720p screenshot embed?)

> 使用多进程结构的理由
1. 仅在AssistiveTouch启动时读入用户配置，Preference对配置进行编辑。（AssistiveTouch结束时存储按钮位置是个例外）
2. Child window的结构会让touch随着游戏窗口的销毁而销毁，部分游戏存在启动器销毁，全屏切换销毁window handle的情景。所以需要常驻一个main进程（TouchChan）来重启AssistiveTouch。因为game handle改变了，必须重新查找handle。这部分逻辑也是依赖入口点进程，AssistiveTouch中依赖的game main handle为只读。
Extensions
3. Gesture lifetime with AssistiveTouch. 模块化，保证功能独立性故与wpf项目分离，AssistiveTouch.Core下的功能已经够多了。自己实现鼠标键盘钩子没有回调的必要？
4. Virtual Keyboard lifetime 由 AssistiveTouch 控制 确实没有回调Touch模拟键盘按下的必要
5. GameController lifetime with AssistiveTouch 所有功能独立，仅接入断开由AssistiveTouch提示。(不要SharpDX太重了) 也许需要XInput1_4.dll 所以不要支持win7
6. Pure danmaku (with textractor inject automatic behind)
UI 怎么办全由自己想办法winform直接画？ 先实现必要可用方便的功能，再考虑优化代码和用户体验本身。还是这个思路。

> .editorconfig changes
1.DllImport (不使用LibraryImport的理由：1.会引入unsafe。2.无法与net472兼容需用宏编译分离两套代码)

> TachiChan使用到的winrt服务，旨在简化开发，不再兼容，抛弃win8前旧平台。（考虑Store应用会导致命令行更难与第三方应用交互）
（c++/winrt Launcher）
1.Appservice与UWP交互
（TouchChan.AssistiveTouch）
2.Toast用来提示是否是管理员模式
3.Battery模块用来检查设备是否带电池（考虑将来替换原生实现）
4.StorageFolder搭配UWP获取程序配置路径


更新履历
1. 更多游戏更好的支持和优化。（不使用Child window，解决触摸会穿透到游戏，全屏看不见Touch，或者游戏窗口无法触摸等）
2. 在小于6寸的设备上1.5倍整体缩放UI
3. 通过环境变量设置游戏的Dpi缩放兼容性，不再写入注册表。
4. (fix) 修复 v1.2.0.1(nightly) 触摸转点击失效的问题

(MASSIVE-Back)Magnifier, I remove these things cause I dont satisfied with them.
(MASSIVE)尝试添加手柄检测，如Ctrl键，Select按键对应透明面板，Start打开Menu（与虚拟键盘一样，一定是独立进程更好，为了减轻touch重量）
(MASSIVE)考虑重写winform的虚拟键盘（考虑是属于game[曾经]还是属于function）
(for fun)unpackage net8 native with winrt without winrt.
(for fun-Silent)No Touch showed and start other key function. (If select start toast)
* Touch->Click
* GameHandlerSupport
* s(Danmaku)
