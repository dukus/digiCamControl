using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class ValuePairEnumerator : BaseFieldClass
    {
        private AsyncObservableCollection<ValuePair> _items;
        public AsyncObservableCollection<ValuePair> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public ValuePairEnumerator()
        {
            Items = new AsyncObservableCollection<ValuePair>();
        }

        public ValuePair Add(ValuePair pair)
        {
            Items.Add(pair);
            return pair;
        }

        public string this[string name]
        {
            get
            {
                ValuePair pair = Get(name);
                if(pair==null)
                {
                    pair = new ValuePair() { Name = name };
                    Items.Add(pair);
                }
                return pair.Value;
            }
            set
            {
                ValuePair pair = Get(name);
                if (pair == null)
                {
                    pair = new ValuePair() { Name = name,Value = value};
                    Items.Add(pair);
                }
                else
                {
                    pair.Value = value;    
                }
            }
        }

        public ValuePair Get(string name)
        {
            return Items.FirstOrDefault(valuePair => valuePair.Name == name);
        }

        public bool ContainName(string name)
        {
            return Items.Any(item => item.Name == name);
        }
    }
}
