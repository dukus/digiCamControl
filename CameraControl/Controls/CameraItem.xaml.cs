using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;

namespace CameraControl.Controls
{
  /// <summary>
  /// Interaction logic for CameraItem.xaml
  /// </summary>
  public partial class CameraItem : UserControl, INotifyPropertyChanged
  {
    public static readonly DependencyProperty CameraDeviceProperty = DependencyProperty.Register("CameraDevice", typeof(ICameraDevice), typeof(CameraItem), (PropertyMetadata)new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(CameraDeviceCallBack)));

    private CameraProperty _cameraProperty;

    public CameraProperty CameraProperty
    {
      get { return _cameraProperty; }
      set
      {
        _cameraProperty = value;
        NotifyPropertyChanged("CameraProperty");
      }
    }

    public ICameraDevice CameraDevice
    {
      get
      {
        return (ICameraDevice) GetValue(CameraDeviceProperty);
        ;
      }
      set
      {
        SetValue(CameraDeviceProperty, value);
      }
    }

    private static void CameraDeviceCallBack(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      CameraItem item = dependencyObject as CameraItem;
      ICameraDevice device = dependencyPropertyChangedEventArgs.NewValue as ICameraDevice;
      if (item != null && device != null)
      {
        item.CameraProperty = ServiceProvider.Settings.CameraProperties.Get(device);
      }

    }

    public CameraItem()
    {
      InitializeComponent();
    }

    #region Implementation of INotifyPropertyChanged

    public virtual event PropertyChangedEventHandler PropertyChanged;

    public virtual void NotifyPropertyChanged(String info)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(info));
      }
    }

    #endregion

  }
}
