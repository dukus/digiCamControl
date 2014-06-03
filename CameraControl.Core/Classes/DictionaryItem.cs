using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
  public class DictionaryItem : BaseFieldClass
  {
    private string _name;
    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        NotifyPropertyChanged("Name");
      }
    }

    private string _value;
    public string Value
    {
      get { return _value; }
      set
      {
        _value = value;
        NotifyPropertyChanged("Value");
      }
    }

    public override string ToString()
    {
      return Name + " - " + Value;
    }

  }
}
