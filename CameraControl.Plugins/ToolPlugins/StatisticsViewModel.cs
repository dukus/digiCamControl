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
        private AsyncObservableCollection<NamedValue<int>> _cameras;
        private AsyncObservableCollection<NamedValue<int>> _sessions;
        private AsyncObservableCollection<NamedValue<string>> _summary;

        public AsyncObservableCollection<NamedValue<string>> Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                RaisePropertyChanged(() => Summary);
            }
        }

        public AsyncObservableCollection<NamedValue<int>> Sessions
        {
            get { return _sessions; }
            set
            {
                _sessions = value;
                RaisePropertyChanged(() => Sessions);
            }
        }

        public AsyncObservableCollection<NamedValue<int>> Cameras
        {
            get { return _cameras; }
            set
            {
                _cameras = value;
                RaisePropertyChanged(() => Cameras);
            }
        }

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
                Cameras = new AsyncObservableCollection<NamedValue<int>>();
                Sessions = new AsyncObservableCollection<NamedValue<int>>();
                Summary = new AsyncObservableCollection<NamedValue<string>>();
                
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

                    dictionary.Clear();
                    foreach (DbFile file in Files)
                    {
                        if(!dictionary.ContainsKey(file.Camera))
                            dictionary.Add(file.Camera,0);
                        dictionary[file.Camera]++;
                    }

                    foreach (var i in dictionary)
                    {
                            Cameras.Add(new NamedValue<int>(i.Key, i.Value));
                    }

                    dictionary.Clear();
                    foreach (DbFile file in Files)
                    {
                        if (!dictionary.ContainsKey(file.Session))
                            dictionary.Add(file.Session, 0);
                        dictionary[file.Session]++;
                    }

                    foreach (var i in dictionary)
                    {
                        Sessions.Add(new NamedValue<int>(i.Key, i.Value));
                    }

                    Summary.Add(new NamedValue<string>("Total photos", Files.Count.ToString()));
                    Summary.Add(new NamedValue<string>("Total used session", Sessions.Count.ToString()));
                    Summary.Add(new NamedValue<string>("Total used cameras", Cameras.Count.ToString()));

                    if (Files.Count > 0)
                    {
                        Summary.Add(new NamedValue<string>("Most used apperture",
                            Files.GroupBy(x => x.F).OrderByDescending(x => x.Count()).First().Key));
                        Summary.Add(new NamedValue<string>("Most used exposure",
                            Files.GroupBy(x => x.E).OrderByDescending(x => x.Count()).First().Key));
                        Summary.Add(new NamedValue<string>("Most used ISO",
                            Files.GroupBy(x => x.Iso).OrderByDescending(x => x.Count()).First().Key));
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
