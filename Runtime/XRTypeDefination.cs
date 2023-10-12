namespace FFalcon.XR.Runtime
{
    using System.Runtime.InteropServices;

    public enum XRStateEvent
    {
        //Mag Calibration
        kStateMagNeedCalibrate = 0x6000,
        kStateMagDoingCalibrate = 0x6001,
        kStateMagCalibrateSuccess = 0x6002,
        kStateMagCalibrateFailed = 0x6003,
    }

    public enum XRControlUnit
    {
        kUnitUnknown = 0,
        kUnitHeadTracker,
    }
    public enum FXRControlCommand
    {
        kCtlCmdUnknown = 0,
        kCtlCmdStartMagCalibration,
        kCtlCmdStopMagCalibration,
    }

    public enum XRPlaneProperty { PLANE_HORIZONTAL_UP, PLANE_HORIZONTAL_DOWN , PLANE_VERTICAL, PLANE_NON };

    [StructLayout(LayoutKind.Sequential)]
    public struct XRRotation
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XRPosition
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XRRange
    {
        public float x;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XRPoseInfo
    {
        public ulong timestamp;
        public XRRotation rotation;
        public XRPosition position;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XRPlaneInfo
    {
        public XRPlaneProperty property;
        public XRPoseInfo pose;
        public XRRange local_range;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public float[] local_polygon;
        public int local_polygon_size;
    }
}