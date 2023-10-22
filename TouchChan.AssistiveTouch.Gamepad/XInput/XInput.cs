// SharpDX.XInput, Version=4.2.0.0, Culture=neutral, PublicKeyToken=b4dcf0f35e5521f1
// SharpDX.XInput.XInput
using System;
using System.Runtime.InteropServices;
using SharpDX.Mathematics.Interop;

namespace SharpDX.XInput;

internal static class XInput
{
    public unsafe static int XInputGetState(int dwUserIndex, out State stateRef)
    {
        stateRef = default(State);
        int result;
        fixed (State* ptr = &stateRef)
        {
            void* param = ptr;
            result = XInputGetState_(dwUserIndex, param);
        }
        return result;
    }

    [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetState")]
    private unsafe static extern int XInputGetState_(int param0, void* param1);

    public unsafe static int XInputSetState(int dwUserIndex, ref Vibration vibrationRef)
    {
        Vibration.__Native @ref = default(Vibration.__Native);
        vibrationRef.__MarshalTo(ref @ref);
        int result = XInputSetState_(dwUserIndex, &@ref);
        vibrationRef.__MarshalFree(ref @ref);
        return result;
    }

    [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputSetState")]
    private unsafe static extern int XInputSetState_(int param0, void* param1);

    public unsafe static int XInputGetCapabilities(int dwUserIndex, DeviceQueryType dwFlags, out Capabilities capabilitiesRef)
    {
        Capabilities.__Native @ref = default(Capabilities.__Native);
        capabilitiesRef = default(Capabilities);
        int result = XInputGetCapabilities_(dwUserIndex, (int)dwFlags, &@ref);
        capabilitiesRef.__MarshalFrom(ref @ref);
        return result;
    }

    [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetCapabilities")]
    private unsafe static extern int XInputGetCapabilities_(int param0, int param1, void* param2);

    public static void XInputEnable(RawBool enable)
    {
        XInputEnable_(enable);
    }

    [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputEnable")]
    private static extern void XInputEnable_(RawBool param0);

    public unsafe static int XInputGetAudioDeviceIds(int dwUserIndex, IntPtr renderDeviceIdRef, IntPtr renderCountRef, IntPtr captureDeviceIdRef, IntPtr captureCountRef)
    {
        return XInputGetAudioDeviceIds_(dwUserIndex, (void*)renderDeviceIdRef, (void*)renderCountRef, (void*)captureDeviceIdRef, (void*)captureCountRef);
    }

    [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetAudioDeviceIds")]
    private unsafe static extern int XInputGetAudioDeviceIds_(int param0, void* param1, void* param2, void* param3, void* param4);

    public unsafe static int XInputGetBatteryInformation(int dwUserIndex, BatteryDeviceType devType, out BatteryInformation batteryInformationRef)
    {
        batteryInformationRef = default(BatteryInformation);
        int result;
        fixed (BatteryInformation* ptr = &batteryInformationRef)
        {
            void* param = ptr;
            result = XInputGetBatteryInformation_(dwUserIndex, (int)devType, param);
        }
        return result;
    }

    [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetBatteryInformation")]
    private unsafe static extern int XInputGetBatteryInformation_(int param0, int param1, void* param2);

    public unsafe static int XInputGetKeystroke(int dwUserIndex, int dwReserved, out Keystroke keystrokeRef)
    {
        keystrokeRef = default(Keystroke);
        int result;
        fixed (Keystroke* ptr = &keystrokeRef)
        {
            void* param = ptr;
            result = XInputGetKeystroke_(dwUserIndex, dwReserved, param);
        }
        return result;
    }

    [DllImport("xinput1_4.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetKeystroke")]
    private unsafe static extern int XInputGetKeystroke_(int param0, int param1, void* param2);
}
