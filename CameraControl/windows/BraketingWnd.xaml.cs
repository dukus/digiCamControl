using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for BraketingWnd.xaml
  /// </summary>
  public partial class BraketingWnd 
  {
    private ICameraDevice _device;
    private PhotoSession _photoSession;
    AsyncObservableCollection<CheckedListItem> collection = new AsyncObservableCollection<CheckedListItem>();
    AsyncObservableCollection<CheckedListItem> shuttercollection = new AsyncObservableCollection<CheckedListItem>();
    AsyncObservableCollection<CheckedListItem> presetcollection = new AsyncObservableCollection<CheckedListItem>();
    BackgroundWorker backgroundWorker = new BackgroundWorker();

    public BraketingWnd(ICameraDevice device, PhotoSession session)
    {
      InitializeComponent();
      _device = device;
      _photoSession = session;
      _photoSession.Braketing.IsBusy = false;
      backgroundWorker.DoWork += delegate { _photoSession.Braketing.TakePhoto(ServiceProvider.DeviceManager.SelectedCameraDevice); };
      _photoSession.Braketing.IsBusyChanged += Braketing_IsBusyChanged;
      _photoSession.Braketing.PhotoCaptured += Braketing_PhotoCaptured;
      _photoSession.Braketing.BracketingDone += Braketing_BracketingDone;
      ServiceProvider.Settings.ApplyTheme(this);
    }

    void Braketing_BracketingDone(object sender, EventArgs e)
    {
      Dispatcher.Invoke(new Action(delegate { lbl_status.Content = TranslationStrings.MsgBracketingDone; }));
    }

    void Braketing_PhotoCaptured(object sender, EventArgs e)
    {
      int count = 0;
      switch (_photoSession.Braketing.Mode)
      {
        case 0:
          count = _photoSession.Braketing.ExposureValues.Count;
          break;
        case 1:
          count = _photoSession.Braketing.ShutterValues.Count;
          break;
        case 2:
          count = _photoSession.Braketing.PresetValues.Count;
          break;

      }
      Dispatcher.Invoke(new Action(delegate
                                     {
                                       lbl_status.Content = string.Format(TranslationStrings.MsgActionInProgress, count, _photoSession.Braketing.Index);
                                     }));
    }

    void Braketing_IsBusyChanged(object sender, EventArgs e)
    {
      Dispatcher.Invoke(new Action(delegate { btn_shot.Content = _photoSession.Braketing.IsBusy ? TranslationStrings.ButtonStop : TranslationStrings.ButtonStart; }));
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (_photoSession.Braketing.ExposureValues.Count == 0)
        _photoSession.Braketing.ExposureValues = new AsyncObservableCollection<string> {"-1", "0", "+1"};
      foreach (string value in _device.ExposureCompensation.Values)
      {
        CheckedListItem item = new CheckedListItem()
                                 {Name = value, IsChecked = _photoSession.Braketing.ExposureValues.Contains(value)};
        item.PropertyChanged += item_PropertyChanged;
        collection.Add(item);
      }
      listBox1.ItemsSource = collection;

      Dictionary<long, string> exposurevals = new Dictionary<long, string>();
      
      int s_index = _device.ShutterSpeed.Values.IndexOf(_device.ShutterSpeed.Value);
      int s_min = s_index - 4;
      int s_max = s_index + 4;
      //exposurevals.Add(s_index, "0");
      //long incre = _device.ShutterSpeed.NumericValues[s_index];
      //int step = 0;
      //for (int i = s_index; i < _device.ShutterSpeed.Values.Count; i++)
      //{
      //  if (_device.ShutterSpeed.NumericValues[i]%incre == 0)
      //  {
      //    exposurevals.Add(i, step.ToString());
      //    step++;
      //    incre = _device.ShutterSpeed.NumericValues[i];
      //  }
      //}

      //incre = _device.ShutterSpeed.NumericValues[s_index];
      //step = 0;
      
      //for (int i = s_index; i >0; i--)
      //{
      //  if (incre % _device.ShutterSpeed.NumericValues[i] == 0)
      //  {
      //    if (step != 0)
      //    {
      //      exposurevals.Add(i, (-step).ToString());
      //    }
      //    step++;
      //    incre = _device.ShutterSpeed.NumericValues[i];
      //  }
      //}

      if (s_min < 0)
      {
        s_min = 0;
      }
      if (s_max >= _device.ShutterSpeed.Values.Count)
        s_max = _device.ShutterSpeed.Values.Count - 1;
      for (int i = 0; i < _device.ShutterSpeed.Values.Count; i++)
      {
        if (_device.ShutterSpeed.Values[i] == "Bulb")
          continue;
        CheckedListItem item = new CheckedListItem()
                                 {
                                   Name =
                                     _device.ShutterSpeed.Values[i] +
                                     (exposurevals.ContainsKey(i) ? " EV " + exposurevals[i] : ""),
                                   Tag = _device.ShutterSpeed.Values[i]
                                 };
        if (i == s_index || i == s_min || i == s_max)
          item.IsChecked = true;
        item.PropertyChanged += item_PropertyChanged;
        shuttercollection.Add(item);
      }
      lst_shutter.ItemsSource = shuttercollection;

      foreach (CameraPreset cameraPreset in ServiceProvider.Settings.CameraPresets)
      {
        CheckedListItem item = new CheckedListItem()
                                 {
                                   Name = cameraPreset.Name,
                                   IsChecked = _photoSession.Braketing.PresetValues.Contains(cameraPreset.Name)
                                 };
        item.PropertyChanged += item_PropertyChanged;
        presetcollection.Add(item);
      }
      lst_preset.ItemsSource = presetcollection;

      if(_device.Mode.Value=="M")
      {
        tab_exposure.Visibility = Visibility.Visible;
        tab_manual.Visibility = Visibility.Visible;
        tab_manual.IsSelected = true;
        _photoSession.Braketing.Mode = 1;
      }
      else
      {
        _photoSession.Braketing.Mode = 0;
        tab_exposure.Visibility = Visibility.Visible;
        tab_manual.Visibility = Visibility.Collapsed;
        tab_exposure.IsSelected = true;
      }
      item_PropertyChanged(null, null);
    }

    void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      _photoSession.Braketing.ExposureValues.Clear();
      foreach (CheckedListItem listItem in collection.Where(listItem => listItem.IsChecked && !_photoSession.Braketing.ExposureValues.Contains(listItem.Name)))
      {
        _photoSession.Braketing.ExposureValues.Add(listItem.Name);
      }
      _photoSession.Braketing.ShutterValues.Clear();
      foreach (CheckedListItem listItem in shuttercollection.Where(listItem => listItem.IsChecked && !_photoSession.Braketing.ShutterValues.Contains(listItem.Name)))
      {
        _photoSession.Braketing.ShutterValues.Add(listItem.Tag);
      }
      _photoSession.Braketing.PresetValues.Clear();
      foreach (CheckedListItem listItem in presetcollection.Where((item) => item.IsChecked && !_photoSession.Braketing.PresetValues.Contains(item.Name)))
      {
        _photoSession.Braketing.PresetValues.Add(listItem.Name);
      }
    }

    private void btn_shot_Click(object sender, RoutedEventArgs e)
    {
      if (!_photoSession.Braketing.IsBusy)
      {
        backgroundWorker.RunWorkerAsync();
      }
      else
      {
        _photoSession.Braketing.Stop();
      }
      
    }

    private void btn_close_Click(object sender, RoutedEventArgs e)
    {
      _photoSession.Braketing.Stop();
      this.Close();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      _photoSession.Braketing.Stop();
    }

    private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if(e.AddedItems[0]==tab_exposure)
      {
        _photoSession.Braketing.Mode = 0;
      }
      if (e.AddedItems[0] == tab_manual)
      {
        _photoSession.Braketing.Mode = 1;
      }
      if (e.AddedItems[0] == tab_preset)
      {
        _photoSession.Braketing.Mode = 2;
      }

    }

    private void btn_help_Click(object sender, RoutedEventArgs e)
    {
      HelpProvider.Run(HelpSections.Bracketig);
    }

    private void btn_reset_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in collection)
        {
            item.IsChecked = false;
        }
        foreach (var item in shuttercollection)
        {
            item.IsChecked = false;
        }
        foreach (var item in presetcollection)
        {
            item.IsChecked = false;
        }
    }


  }
}
