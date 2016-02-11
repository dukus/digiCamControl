using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Database;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CameraControl.Plugins.ToolPlugins
{
    public class StatisticsViewModel : ViewModelBase
    {
        private DateTime _from;
        private DateTime _to;
        

        private ObservableCollection<DbEvents> _apps;
        private ObservableCollection<DbFile> _files;
        private AsyncObservableCollection<NamedValue<int>> _formats;

        public AsyncObservableCollection<NamedValue<int>> Formats
        {
            get { return _formats; }
            set
            {
                _formats = value;
                RaisePropertyChanged(() => Formats);
            }
        }

        public ObservableCollection<DbEvents> Apps
        {
            get { return _apps; }
            set
            {
                _apps = value;
                RaisePropertyChanged(() => Apps);
            }
        }

        public ObservableCollection<DbFile> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                RaisePropertyChanged(() => Files);
            }
        }

        public DateTime From
        {
            get { return _from; }
            set
            {
                _from = value;
                RaisePropertyChanged(() => From);
                Refresh();
            }
        }

        public DateTime To
        {
            get { return _to; }
            set
            {
                _to = value;
                RaisePropertyChanged(() => To);
                Refresh();
            }
        }

        public RelayCommand RefreshCommand { get; set; }

        public StatisticsViewModel()
        {
            To = DateTime.Now;
            From = DateTime.Now.AddMonths(-1);
            RefreshCommand = new RelayCommand(Refresh);
        }

        private void Refresh()
        {
            try
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                
                Formats = new AsyncObservableCollection<NamedValue<int>>();
                
                if (!IsInDesignMode)
                {
                    Apps = new ObservableCollection<DbEvents>(ServiceProvider.Database.GetApp(From, To));
                    Files = new ObservableCollection<DbFile>(ServiceProvider.Database.GetFiles(From, To));
                    
                    dictionary.Add("Jpg",0);
                    dictionary.Add("Raw", 0);
                    dictionary.Add("Video", 0);
                    dictionary.Add("Other", 0);

                    foreach (var file in Files)
                    {
                        var item=new FileItem(file.File);
                        if (item.IsJpg)
                            dictionary["Jpg"]++;
                        else if (item.IsRaw)
                        {
                            dictionary["Raw"]++;
                        }
                        else if (item.IsMovie)
                        {
                            dictionary["Video"]++;
                        }
                        else
                        {
                            dictionary["Other"]++;
                        }
                    }
                    foreach (var i in dictionary)
                    {
                        if(i.Value>0)
                        Formats.Add(new NamedValue<int>(i.Key,i.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
            }
            
        }
    }
}
