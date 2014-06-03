using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
  public class CameraPropertyEnumerator
  {
    public AsyncObservableCollection<CameraProperty> Items { get; set; }

    public CameraPropertyEnumerator()
    {
      Items = new AsyncObservableCollection<CameraProperty>();
    }

    public CameraProperty Get(ICameraDevice device)
    {
      if (device == null)
        return new CameraProperty();
      foreach (CameraProperty cameraProperty in Items)
      {
        if (cameraProperty.SerialNumber == device.SerialNumber)
          return cameraProperty;
      }
      var c = new CameraProperty() {SerialNumber = device.SerialNumber, DeviceName = device.DisplayName};
      Items.Add(c);
      return c;
    }

  }
}
