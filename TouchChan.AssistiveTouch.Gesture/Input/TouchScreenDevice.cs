using TouchChan.AssistiveTouch.Gesture.Common;
using TouchChan.AssistiveTouch.Gesture.Native;

namespace TouchChan.AssistiveTouch.Gesture.Input;

public class TouchScreenDevice : HidDevice
{
    public override Devices DeviceType => Devices.TouchScreen;

    public TouchScreenDevice(nint rawInputBuffer, ref RAWINPUT raw) : base(rawInputBuffer, ref raw)
    {
    }

    public void GetRawDatas(short numberOfChildren, Screen currentScr, ref int requiringContactCount, ref List<RawData> _outputTouchs)
    {
        for (int dwIndex = 0; dwIndex < _dwCount; dwIndex++)
        {
            var pRawDataPacket = new IntPtr(_pRawData.ToInt64() + dwIndex * _dwSizHid);
            for (short nodeIndex = 1; nodeIndex <= numberOfChildren; nodeIndex++)
            {
                int contactIdentifier = GetContactId(nodeIndex, pRawDataPacket);
                Point point = GetCoordinate(nodeIndex, currentScr, pRawDataPacket);

                ushort[] usageList = GetButtonList(_hPreparsedData.DangerousGetHandle(), _pRawData, nodeIndex, _dwSizHid);
                bool tip = usageList.Length != 0 && usageList[0] == NativeMethods.TipId;

                _outputTouchs.Add(new RawData(tip ? DeviceStates.Tip : DeviceStates.None, contactIdentifier, point));

                if (--requiringContactCount == 0) break;
            }
            if (requiringContactCount == 0) break;
        }
    }
}
