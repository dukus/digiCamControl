using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
  public class TagItem:BaseFieldClass
  {
    private string _displayValue;
    public string DisplayValue
    {
      get { return _displayValue; }
      set
      {
        _displayValue = value;
        NotifyPropertyChanged("DisplayValue");
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

    private bool _tag1Checked;
    public bool Tag1Checked
    {
      get { return _tag1Checked; }
      set
      {
        _tag1Checked = value;
        NotifyPropertyChanged("Tag1Checked");
      }
    }

    private bool _tag2Checked;
    public bool Tag2Checked
    {
      get { return _tag2Checked; }
      set
      {
        _tag2Checked = value;
        NotifyPropertyChanged("Tag2Checked");
      }
    }

    private bool _tag3Checked;
    public bool Tag3Checked
    {
      get { return _tag3Checked; }
      set
      {
        _tag3Checked = value;
        NotifyPropertyChanged("Tag3Checked");
      }
    }

    private bool _tag4Checked;
    public bool Tag4Checked
    {
      get { return _tag4Checked; }
      set
      {
        _tag4Checked = value;
        NotifyPropertyChanged("Tag4Checked");
      }
    }

    public TagItem()
    {
      Value = "";
      DisplayValue = "";
      Tag1Checked = false;
      Tag2Checked = false;
      Tag3Checked = false;
      Tag4Checked = false;
    }

    public override string ToString()
    {
      return DisplayValue + " - " + Value;
    }
  }
}
