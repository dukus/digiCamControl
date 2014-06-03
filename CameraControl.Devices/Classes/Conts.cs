using System.Runtime.InteropServices;

namespace CameraControl.Devices.Classes
{
  public class Conts
  {
    [MarshalAs(UnmanagedType.LPStr)]
    public const string wiaEventDeviceConnected = "{A28BBADE-64B6-11D2-A231-00C04FA31809}";
    [MarshalAs(UnmanagedType.LPStr)]
    public const string wiaEventDeviceDisconnected = "{143E4E83-6497-11D2-A231-00C04FA31809}";
    [MarshalAs(UnmanagedType.LPStr)]
    public const string wiaEventItemCreated = "{4C8F4EF5-E14F-11D2-B326-00C04F68CE61}";
    [MarshalAs(UnmanagedType.LPStr)]
    public const string wiaEventItemDeleted = "{1D22A559-E14F-11D2-B326-00C04F68CE61}";
    [MarshalAs(UnmanagedType.LPStr)]
    public const string wiaCommandTakePicture = "{AF933CAC-ACAD-11D2-A093-00C04F72DC3C}";

    public const string CONST_PROP_F_Number = "F Number";
    public const string CONST_PROP_ISO_Number = "Exposure Index";
    public const string CONST_PROP_Exposure_Time = "Exposure Time";
    public const string CONST_PROP_WhiteBalance = "White Balance";
    public const string CONST_PROP_ExposureMode = "Exposure Mode";
    public const string CONST_PROP_ExposureCompensation = "Exposure Compensation";
    public const string CONST_PROP_BatteryStatus = "Battery Status";
    public const string CONST_PROP_CompressionSetting = "Compression Setting";
    public const string CONST_PROP_ExposureMeteringMode = "Exposure Metering Mode";
    public const string CONST_PROP_FocusMode = "Focus Mode";
  }
}
