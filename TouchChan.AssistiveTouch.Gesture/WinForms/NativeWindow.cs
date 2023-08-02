#if !NET472
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TouchChan.AssistiveTouch.Gesture.Native;

namespace TouchChan.AssistiveTouch.Gesture.WinForms
{
    public class NativeWindow
    {
        public IntPtr Handle { get; private set; }

        public virtual void CreateHandle(CreateParams cp, WindowsProc callback)
        {
            var WindowClass = "HelperWindowClass";
            var wind_class = new WNDCLASS
            {
                lpszClassName = Marshal.StringToHGlobalUni(WindowClass),
                lpfnWndProc = callback
            };

            ushort classAtom = RegisterClassW(ref wind_class);

            if (classAtom == 0)
                throw new Win32Exception();

            const uint WS_EX_NOACTIVATE = 0x08000000;
            const uint WS_POPUP = 0x80000000;
            IntPtr hWnd = CreateWindowExW(
                WS_EX_NOACTIVATE,
                WindowClass,
                "",
                WS_POPUP,
                0, 0, 0, 0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            if (hWnd == IntPtr.Zero)
                throw new Win32Exception();

            Handle = hWnd;
        }

        public virtual void DestroyHandle()
        {
            lock (this)
            {
                if (Handle != IntPtr.Zero)
                {
                    //if (InteropMethods.DestroyWindow(this).IsFalse())
                    //{

                    //    // Now post a close and let it do whatever it needs to do on its own.
                    //    User32.PostMessageW(this, User32.WM.CLOSE);
                    //}

                    Handle = IntPtr.Zero;
                }

                // Now that we have disposed, there is no need to finalize us any more.
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnHandleChange()
        {
        }
        protected virtual void WndProc(ref Message m)
        {
            DefWndProc(ref m);
        }

        public void DefWndProc(ref Message m)
        {
            m.Result = DefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam);
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern ushort RegisterClassW([In] ref WNDCLASS lpWndClass);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowExW(uint dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            uint dwStyle, int x, int y, int nWidth, int nHeight,
            IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);


        public delegate IntPtr WindowsProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct WNDCLASS
        {
            public uint style;
            public WindowsProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public nint lpszClassName;
        }
    }

    public class CreateParams
    {
        /// <summary>
        ///  Name of the window class to subclass. The default value for this field
        ///  is null, indicating that the window is not a subclass of an existing
        ///  window class. To subclass an existing window class, store the window
        ///  class name in this field. For example, to subclass the standard edit
        ///  control, set this field to "EDIT".
        /// </summary>
        public string? ClassName { get; set; }

        /// <summary>
        ///  The initial caption your control will have.
        /// </summary>
        public string? Caption { get; set; }

        /// <summary>
        ///  Window style bits. This must be a combination of WS_XXX style flags and
        ///  other control-specific style flags.
        /// </summary>
        public int Style { get; set; }

        /// <summary>
        ///  Extended window style bits. This must be a combination of WS_EX_XXX
        ///  style flags.
        /// </summary>
        public int ExStyle { get; set; }

        /// <summary>
        ///  Class style bits. This must be a combination of CS_XXX style flags. This
        ///  field is ignored if the className field is not null.
        /// </summary>
        public int ClassStyle { get; set; }

        /// <summary>
        ///  The left portion of the initial proposed location.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///  The top portion of the initial proposed location.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        ///  The initially proposed width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///  The initially proposed height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///  The controls parent.
        /// </summary>
        public IntPtr Parent { get; set; }

        /// <summary>
        ///  Any extra information that the underlying handle might want.
        /// </summary>
        public object? Param { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder(64);
            builder.Append("CreateParams {'");
            builder.Append(ClassName);
            builder.Append("', '");
            builder.Append(Caption);
            builder.Append("', 0x");
            builder.Append(Convert.ToString(Style, 16));
            builder.Append(", 0x");
            builder.Append(Convert.ToString(ExStyle, 16));
            builder.Append(", {");
            builder.Append(X);
            builder.Append(", ");
            builder.Append(Y);
            builder.Append(", ");
            builder.Append(Width);
            builder.Append(", ");
            builder.Append(Height);
            builder.Append('}');
            builder.Append('}');
            return builder.ToString();
        }
    }

    public struct Message
    {
        public nint HWnd { readonly get; set; }
        public int Msg { get; set; }
        public nint WParam { get; set; }
        public nint LParam { get; set; }
        public nint Result { get; set; }
    }
}
#endif