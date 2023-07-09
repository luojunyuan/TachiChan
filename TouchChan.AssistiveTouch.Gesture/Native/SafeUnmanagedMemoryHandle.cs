﻿using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace TouchChan.AssistiveTouch.Gesture.Native;

[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
public sealed class SafeUnmanagedMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    // Set ownsHandle to true for the default constructor.
    public SafeUnmanagedMemoryHandle() : base(true) { }

    // Set the handle and set ownsHandle to true.
    public SafeUnmanagedMemoryHandle(nint preexistingHandle, bool ownsHandle = true)
        : base(ownsHandle)
    {
        SetHandle(preexistingHandle);
    }

    // Perform any specific actions to release the 
    // handle in the ReleaseHandle method.
    // Often, you need to use Pinvoke to make
    // a call into the Win32 API to release the 
    // handle. In this case, however, we can use
    // the Marshal class to release the unmanaged
    // memory.
    override protected bool ReleaseHandle()
    {
        // "handle" is the internal
        // value for the IntPtr handle.

        // If the handle was set,
        // free it. Return success.
        if (handle != nint.Zero)
        {

            // Free the handle.
            Marshal.FreeHGlobal(handle);

            // Set the handle to zero.
            handle = nint.Zero;

            // Return success.
            return true;
        }

        // Return false. 
        return false;
    }
}
