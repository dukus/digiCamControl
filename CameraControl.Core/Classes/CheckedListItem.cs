using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
  public class CheckedListItem : BaseFieldClass
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Tag { get; set; }
    private bool _isChecked;

    public bool IsChecked
    {
      get { return _isChecked; }
      set
      {
        _isChecked = value;
        NotifyPropertyChanged("IsChecked");
      }
    }
  }
}
