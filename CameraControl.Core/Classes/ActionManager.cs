using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
  public class ActionManager : BaseFieldClass
  {
    private AsyncObservableCollection<IMenuAction> _actions;

    public AsyncObservableCollection<IMenuAction> Actions
    {
      get { return _actions; }
      set
      {
        _actions = value;
        NotifyPropertyChanged("Actions");
      }
    }

    public ActionManager()
    {
      Actions = new AsyncObservableCollection<IMenuAction>();
                  ;
    }
  }
}
