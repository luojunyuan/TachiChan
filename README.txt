Code Name: TouchChan
表紙ネーム：TachiChan - タッチちゃん - 触控酱

### build
* Universal Windows Platform Devolopment (about 12gb)
Preference (UWP)
TouchChanPackage
* C++ Desktop Devolopment (MSVC v14* C++ x86/x64/arm64 build tools about 3gb)
WinRTLauncher

### debug
Debug -> x64 -> Package -> "Run"

### publish
Release -> Package -> "Publish Application"

##### Find My Future
More test on different devices

使用多进程结构的理由
1. 仅在AssistiveTouch启动时读入用户配置，Preference对配置进行编辑。（AssistiveTouch结束时存储按钮位置是个例外）
2. Child window的结构会让touch随着游戏窗口的销毁而销毁，部分游戏存在启动器销毁，全屏切换销毁window handle的情景。所以需要常驻一个main进程（TouchChan）来重启AssistiveTouch。因为game handle改变了，必须重新查找handle。这部分逻辑也是依赖入口点进程，AssistiveTouch中依赖的game main handle为只读。
3. Gesture 引用了winforms，为保持纯净必须与wpf项目分离开。更多的是为了模块化，保证功能独立性，AssistiveTouch.Core下的功能已经够多了。

.editorconfig changes
1.DllImport (不使用LibraryImport的理由：1.会引入unsafe。2.无法与net472兼容需用宏编译分离两套代码)

TachiChan使用到的winrt服务
（c++/winrt Launcher）
1.Appservice与UWP交互
（TouchChan.AssistiveTouch）
2.Toast用来提示是否是管理员模式
3.Battery模块用来检查设备是否带电池（考虑将来替换原生实现）
4.StorageFolder搭配UWP获取程序配置

优先级与TODO
1. 比起鼠标操作电视桌面等环境优先考虑触摸操作平板手机掌机等小型设备
2. 考察通过设备尺寸和分辨率与DPI来设置Touch大小的可靠性
3. 增加说明书以及首次使用时手势操作说明
