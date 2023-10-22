using SharpDX.Win32;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX.XInput
{
    [Flags]
    public enum CapabilityFlags : short
    {
        VoiceSupported = 4,
        FfbSupported = 1,
        Wireless = 2,
        PmdSupported = 8,
        NoNavigation = 0x10,
        None = 0
    }

    public struct Capabilities
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct __Native
        {
            public DeviceType Type;

            public DeviceSubType SubType;

            public CapabilityFlags Flags;

            public Gamepad Gamepad;

            public Vibration.__Native Vibration;
        }

        public DeviceType Type;

        public DeviceSubType SubType;

        public CapabilityFlags Flags;

        public Gamepad Gamepad;

        public Vibration Vibration;

        internal void __MarshalFree(ref __Native @ref)
        {
            Vibration.__MarshalFree(ref @ref.Vibration);
        }

        internal void __MarshalFrom(ref __Native @ref)
        {
            Type = @ref.Type;
            SubType = @ref.SubType;
            Flags = @ref.Flags;
            Gamepad = @ref.Gamepad;
            Vibration.__MarshalFrom(ref @ref.Vibration);
        }

        internal void __MarshalTo(ref __Native @ref)
        {
            @ref.Type = Type;
            @ref.SubType = SubType;
            @ref.Flags = Flags;
            @ref.Gamepad = Gamepad;
            Vibration.__MarshalTo(ref @ref.Vibration);
        }
    }

    public struct Vibration
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct __Native
        {
            public short LeftMotorSpeed;

            public short RightMotorSpeed;
        }

        public ushort LeftMotorSpeed;

        public ushort RightMotorSpeed;

        internal void __MarshalFree(ref __Native @ref)
        {
        }

        internal void __MarshalFrom(ref __Native @ref)
        {
            LeftMotorSpeed = (ushort)@ref.LeftMotorSpeed;
            RightMotorSpeed = (ushort)@ref.RightMotorSpeed;
        }

        internal void __MarshalTo(ref __Native @ref)
        {
            @ref.LeftMotorSpeed = (short)LeftMotorSpeed;
            @ref.RightMotorSpeed = (short)RightMotorSpeed;
        }
    }


    [Flags]
    public enum GamepadKeyCode : short
    {
        A = 0x5800,
        B = 0x5801,
        X = 0x5802,
        Y = 0x5803,
        RightShoulder = 0x5804,
        LeftShoulder = 0x5805,
        LeftTrigger = 0x5806,
        RightTrigger = 0x5807,
        DPadUp = 0x5810,
        DPadDown = 0x5811,
        DPadLeft = 0x5812,
        DPadRight = 0x5813,
        Start = 0x5814,
        Back = 0x5815,
        LeftThumbPress = 0x5816,
        RightThumbPress = 0x5817,
        LeftThumbUp = 0x5820,
        LeftThumbDown = 0x5821,
        LeftThumbRight = 0x5822,
        LeftThumbLeft = 0x5823,
        RightThumbUpLeft = 0x5824,
        LeftThumbUpright = 0x5825,
        LeftThumbDownright = 0x5826,
        RightThumbDownLeft = 0x5827,
        RightThumbUp = 0x5830,
        RightThumbDown = 0x5831,
        RightThumbRight = 0x5832,
        RightThumbLeft = 0x5833,
        RightThumbUpleft = 0x5834,
        RightThumbUpRight = 0x5835,
        RightThumbDownRight = 0x5836,
        RightThumbDownleft = 0x5837,
        None = 0
    }

    [Flags]
    public enum KeyStrokeFlags : short
    {
        KeyDown = 1,
        KeyUp = 2,
        Repeat = 4,
        None = 0
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Keystroke
    {
        public GamepadKeyCode VirtualKey;

        public char Unicode;

        public KeyStrokeFlags Flags;

        public UserIndex UserIndex;

        public byte HidCode;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct State
    {
        public int PacketNumber;

        public Gamepad Gamepad;
    }

    public enum DeviceType : byte
    {
        Gamepad = 1
    }
    public enum DeviceSubType : byte
    {
        Gamepad = 1,
        Unknown = 0,
        Wheel = 2,
        ArcadeStick = 3,
        FlightStick = 4,
        DancePad = 5,
        Guitar = 6,
        GuitarAlternate = 7,
        DrumKit = 8,
        GuitarBass = 11,
        ArcadePad = 19
    }
    public enum DeviceQueryType
    {
        Gamepad = 1,
        Any = 0
    }
    public enum BatteryDeviceType
    {
        Gamepad,
        Headset
    }

    public struct BatteryInformation
    {
        //
        // 概要:
        //     The type of battery. BatteryType will be one of the following values. ValueDescription
        //     BATTERY_TYPE_DISCONNECTEDThe device is not connected.? BATTERY_TYPE_WIREDThe
        //     device is a wired device and does not have a battery.? BATTERY_TYPE_ALKALINEThe
        //     device has an alkaline battery.? BATTERY_TYPE_NIMHThe device has a nickel metal
        //     hydride battery.? BATTERY_TYPE_UNKNOWNThe device has an unknown battery type.?
        //     ?
        public BatteryType BatteryType;

        //
        // 概要:
        //     The charge state of the battery. This value is only valid for wireless devices
        //     with a known battery type. BatteryLevel will be one of the following values.
        //     Value BATTERY_LEVEL_EMPTY BATTERY_LEVEL_LOW BATTERY_LEVEL_MEDIUM BATTERY_LEVEL_FULL
        //     ?
        public BatteryLevel BatteryLevel;
    }
    public enum BatteryLevel : byte
    {
        //
        // 概要:
        //     No documentation.
        Empty,
        //
        // 概要:
        //     No documentation.
        Low,
        //
        // 概要:
        //     No documentation.
        Medium,
        //
        // 概要:
        //     No documentation.
        Full
    }
    public enum BatteryType : byte
    {
        //
        // 概要:
        //     The type of battery. BatteryType will be one of the following values. ValueDescription
        //     BATTERY_TYPE_DISCONNECTEDThe device is not connected.? BATTERY_TYPE_WIREDThe
        //     device is a wired device and does not have a battery.? BATTERY_TYPE_ALKALINEThe
        //     device has an alkaline battery.? BATTERY_TYPE_NIMHThe device has a nickel metal
        //     hydride battery.? BATTERY_TYPE_UNKNOWNThe device has an unknown battery type.?
        //     ?
        Disconnected = 0,
        //
        // 概要:
        //     The charge state of the battery. This value is only valid for wireless devices
        //     with a known battery type. BatteryLevel will be one of the following values.
        //     Value BATTERY_LEVEL_EMPTY BATTERY_LEVEL_LOW BATTERY_LEVEL_MEDIUM BATTERY_LEVEL_FULL
        //     ?
        Wired = 1,
        //
        // 概要:
        //     No documentation.
        Alkaline = 2,
        //
        // 概要:
        //     No documentation.
        Nimh = 3,
        //
        // 概要:
        //     No documentation.
        Unknown = byte.MaxValue
    }

}
