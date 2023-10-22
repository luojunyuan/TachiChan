using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX.Win32
{
    public enum UserIndex : byte
    {
        //
        // 概要:
        //     [in] Index of the signed-in gamer associated with the device. Can be a value
        //     in the range 0?XUSER_MAX_COUNT ? 1, or XUSER_INDEX_ANY to fetch the next available
        //     input event from any user.
        Any = byte.MaxValue,
        //
        // 概要:
        //     [in] Reserved
        One = 0,
        //
        // 概要:
        //     [out] Pointer to an SharpDX.XInput.Keystroke structure that receives an input
        //     event.
        Two = 1,
        //
        // 概要:
        //     No documentation.
        Three = 2,
        //
        // 概要:
        //     No documentation.
        Four = 3
    }
}
