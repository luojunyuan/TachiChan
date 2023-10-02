namespace TouchChan.AssistiveTouch.NativeMethods
{
    /// <summary>
    /// Wrapper of SendInput
    /// </summary>
    internal partial class Simulate
    {
        // Keyboard 

        public static void Click(params KeyCode[] keys)
        {
            foreach (var item in keys)
            {
                KeyDown(item);
                KeyUp(item);
            }
        }

        // Alt(down) Enter(down) Enter(Up) Alt(Up)
        public static void ClickChord(params KeyCode[] keys)
        {
            foreach (var item in keys)
                KeyDown(item);

            foreach (var item in keys.Reverse())
                KeyUp(item);
        }

        public static void Hold(params KeyCode[] keys)
        {
            foreach (var item in keys)
                KeyDown(item);
        }

        public static void Release(params KeyCode[] keys)
        {
            foreach (var item in keys)
                KeyUp(item);
        }


        // Mouse

        public static void Click(params ButtonCode[] keys)
        {
            foreach (var item in keys)
            {
                ButtonDown(item);
                ButtonUp(item);
            }
        }

        public static void Click(ButtonCode key, int delay = 0)
        {
            ButtonDown(key);
            if (delay > 0) Thread.Sleep(delay);
            ButtonUp(key);
        }

        public static void Scroll(ButtonCode code, ButtonScrollDirection direction)
        {
            var offset = (int)direction * DefaultScrollOffset;

            var input = SetMouseEvent(offset, code.ToMouseWheel());
            INPUT[] inputs = new INPUT[] { input };
            SendInput((uint)inputs.Length, inputs, INPUT.Size);
        }



        // Behind

        private static void ButtonDown(ButtonCode code)
        {
            var input = SetMouseEvent((int)code.ToMouseButtonData(), code.ToMouseButtonDownFlags());
            INPUT[] inputs = new INPUT[] { input };
            SendInput((uint)inputs.Length, inputs, INPUT.Size);
        }

        private static void ButtonUp(ButtonCode code)
        {
            var input = SetMouseEvent((int)code.ToMouseButtonData(), code.ToMouseButtonUpFlags());
            INPUT[] inputs = new INPUT[] { input };
            SendInput((uint)inputs.Length, inputs, INPUT.Size);
        }

        private static void KeyDown(KeyCode code)
        {
            // flag = extend ? KeyboardFlag.Extended : KeyboardFlag.None

            var keyEventList = new INPUT[1];
            SetKeyEvent(0, keyEventList, code, KeyboardFlag.None, UIntPtr.Zero);
            SendInput(1, keyEventList, INPUT.Size);
        }

        private static void KeyUp(KeyCode code)
        {
            var keyEventList = new INPUT[1];
            SetKeyEvent(0, keyEventList, code, KeyboardFlag.KeyUp, UIntPtr.Zero);
            SendInput(1, keyEventList, INPUT.Size);
        }

        public static void SetKeyEvent(int i, INPUT[] keyEventArray, KeyCode keyCode, KeyboardFlag flags, nuint extraInfo)
        {
            keyEventArray[i].Type = InputType.Keyboard;
            keyEventArray[i].Data.Keyboard.KeyCode = keyCode;
            keyEventArray[i].Data.Keyboard.Flags = flags;
            keyEventArray[i].Data.Keyboard.ExtraInfo = extraInfo;

            const int MAPVK_VK_TO_VSC = 0;
            keyEventArray[i].Data.Keyboard.ScanCode = (ushort)(MapVirtualKey((ushort)keyCode, MAPVK_VK_TO_VSC) & 0xFFU);
        }

        private const int DefaultScrollOffset = 120;

        private static INPUT SetMouseEvent(int data, MOUSEEVENTF flags) => new INPUT
        {
            Type = InputType.INPUT_MOUSE,
            Data = new InputUnion
            {
                mi = new MOUSEINPUT
                {
                    mouseData = data,
                    dwFlags = flags,
                }
            }
        };
    }
}
