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

现在需要在ErogeHelper主程序上，对winrt和native进行一个取舍
如果选择native，appserve根本无法启用，除非“另寻出路”
选择winrt，目前可行的解决方法，尝试r2r编译
