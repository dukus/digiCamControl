using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Core.Wpf;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using FileInfo = System.IO.FileInfo;
using HelpProvider = CameraControl.Classes.HelpProvider;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;

namespace CameraControl.windows
{
  /// <summary>
  /// Interaction logic for DownloadPhotosWnd.xaml
  /// </summary>
  public partial class DownloadPhotosWnd_old : INotifyPropertyChanged, IWindow
  {
    private bool delete;
    private ProgressWindow dlg = new ProgressWindow();

    private ICameraDevice _cameraDevice;
    public ICameraDevice CameraDevice
    {
      get { return _cameraDevice; }
      set
      {
        _cameraDevice = value;
        NotifyPropertyChanged("CameraDevice");
      }
    }

    private AsyncObservableCollection<FileItem> _items;
    public AsyncObservableCollection<FileItem> Items
    {
      get { return _items; }
      set
      {
        _items = value;
        NotifyPropertyChanged("Items");
      }
    }

    public DownloadPhotosWnd_old()
    {
      InitializeComponent();
      Items = new AsyncObservableCollection<FileItem>();
    }

    private void btn_help_Click(object sender, RoutedEventArgs e)
    {
      HelpProvider.Run(HelpSections.DownloadPhotos);
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

    #region Implementation of IWindow

    public void ExecuteCommand(string cmd, object param)
    {
      switch (cmd)
      {
        case WindowsCmdConsts.DownloadPhotosWnd_Show:
          Dispatcher.Invoke(new Action(delegate
                                         {
                                           if (dlg.IsVisible)
                                             return;
                                           CameraDevice = param as ICameraDevice;
                                           Title = TranslationStrings.DownloadWindowTitle + "-" + ServiceProvider.Settings.CameraProperties.Get(CameraDevice).DeviceName;
                                           if (param == null)
                                             return;
                                           Show();
                                           Activate();
                                           Topmost = true;
                                           Topmost = false;
                                           Focus();
                                           PopulateImageList();
                                         }));
          break;
        case WindowsCmdConsts.DownloadPhotosWnd_Hide:
          Hide();
          break;
        case CmdConsts.All_Close:
          Dispatcher.Invoke(new Action(delegate
          {
            Hide();
            Close();
          }));
          break;
      } 
    }

    #endregion

    private void MetroWindow_Closing(object sender, CancelEventArgs e)
    {
      if (IsVisible)
      {
        e.Cancel = true;
        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.DownloadPhotosWnd_Hide);
      }
    }

    private void PopulateImageList()
    {
      Items.Clear();
      try
      {
        var images = CameraDevice.GetObjects(null);
        foreach (DeviceObject deviceObject in images)
        {
          Items.Add(new FileItem(deviceObject, CameraDevice));
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(TranslationStrings.LabelErrorLoadingFileList);
        Log.Error("Error loading file list", exception);
      }
    }

    private void btn_download_Click(object sender, RoutedEventArgs e)
    {
      if (chk_delete.IsChecked == true && MessageBox.Show(TranslationStrings.LabelAskForDelete, "", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
        return;
      dlg.Show();
      delete = chk_delete.IsChecked == true;
      ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.DownloadPhotosWnd_Hide);
      Thread thread = new Thread(TransferFiles); 
      thread.Start();
    }

    void TransferFiles()
    {
      DateTime starttime = DateTime.Now;
      long totalbytes = 0;
      AsyncObservableCollection<FileItem> itemstoExport = new AsyncObservableCollection<FileItem>(Items.Where(x => x.IsChecked));
      dlg.MaxValue = itemstoExport.Count;
      dlg.Progress = 0;
      int i = 0;
      PhotoSession session = (PhotoSession)CameraDevice.AttachedPhotoSession ?? ServiceProvider.Settings.DefaultSession;
      foreach (FileItem fileItem in itemstoExport)
      {
        dlg.Label = fileItem.FileName;
        dlg.ImageSource = fileItem.Thumbnail;
        string fileName = Path.Combine(session.Folder, fileItem.FileName);
        if (File.Exists(fileName))
          fileName =
            StaticHelper.GetUniqueFilename(
              Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + "_", 0,
              Path.GetExtension(fileName));
        CameraDevice.TransferFile(fileItem.DeviceObject.Handle, fileName);
        // double check if file was transferred
        if (File.Exists(fileName) && delete)
        {
          CameraDevice.DeleteObject(fileItem.DeviceObject);
        }
        totalbytes += new FileInfo(fileName).Length;
        session.AddFile(fileName);
        i++;
        dlg.Progress = i;
      }
      dlg.Hide();
      double transfersec = (DateTime.Now - starttime).TotalSeconds;
      Log.Debug(string.Format("[BENCHMARK]Total byte transferred ;{0}\nTotal seconds :{1}\nSpeed : {2} Mbyte/sec ", totalbytes,
                                    transfersec, (totalbytes/transfersec/1024/1024).ToString("0000.00")));
    }
  }
}
