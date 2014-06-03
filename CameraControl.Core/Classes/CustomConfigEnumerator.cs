using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class CustomConfigEnumerator : BaseFieldClass
    {
        private AsyncObservableCollection<CustomConfig> _items;
        public AsyncObservableCollection<CustomConfig> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
            
        }

        public CustomConfigEnumerator()
        {
            Items=new AsyncObservableCollection<CustomConfig>();
        }

        public CustomConfig Get(string name)
        {
            return Items.FirstOrDefault(customConfig => customConfig.Name == name);
        }
    }
}
