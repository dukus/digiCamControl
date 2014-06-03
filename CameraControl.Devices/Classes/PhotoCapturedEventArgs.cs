using WIA;

namespace CameraControl.Devices.Classes
{
  public delegate void PhotoCapturedEventHandler(object sender, PhotoCapturedEventArgs eventArgs);

  public class PhotoCapturedEventArgs
  {
    private Item _wiaImageItem;

    public Item WiaImageItem
    {
      get { return _wiaImageItem; }
      set
      {
        _wiaImageItem = value;
        //ImageFile = (ImageFile)WiaImageItem.Transfer("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}");
      }
    }

    public ImageFile ImageFile { get; set; }

    public bool Transfer(string fileName)
    {
      //imageFile.SaveFile(fileName);
      return true;
    }

    public object EventArgs { get; set; }

    public ICameraDevice CameraDevice { get; set; }
    public string FileName { get; set; }
    public object Handle { get; set; }

  }
}
