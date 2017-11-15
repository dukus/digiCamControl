using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Database;
using GalaSoft.MvvmLight;

namespace Capture.Workflow.Core.Classes
{
    public class QueueManager : ViewModelBase
    {
        private bool _shouldStop;
        private int _count;
        private string _errorMessage;
        private static QueueManager _instance;
        private object _sync = new object();
        private bool _isActive;

        public static QueueManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QueueManager();
                    //_instance.Start();
                }
                return _instance;
            }
            set { _instance = value; }
        }

        public AsyncObservableCollection<DbQueue> Items { get; set; }

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;

                IsActive = Count > 0;
                RaisePropertyChanged(() => Count);
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }


        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        public QueueManager()
        {
            Items = new AsyncObservableCollection<DbQueue>();

        }

        public void Add(DbQueue item)
        {
            Items.Add(item);
        }

        public void Start()
        {
            Task.Factory.StartNew(Process);
        }

        public void Process()
        {
            while (true)
            {
                try
                {
                    var items = WorkflowManager.Instance.Database.GetList();
                    Count = items.Count;
                    foreach (var item in items)
                    {

                        if (item.Done == true)
                        {
                            Count--;
                            ErrorMessage = "";
                        }
                        WorkflowManager.Instance.Database.Save(item);
                        Thread.Sleep(150);
                        if (_shouldStop)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    Log.Error("Queue error", ex);
                }
                if (_shouldStop)
                {
                    break;
                }
                Thread.Sleep(500);
            }
        }



        public void Stop()
        {
            _shouldStop = true;
        }




    }
}
