#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices.Classes;

#endregion

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
                if (pair == null)
                {
                    pair = new ValuePair() {Name = name};
                    Items.Add(pair);
                }
                return pair.Value;
            }
            set
            {
                ValuePair pair = Get(name);
                if (pair == null)
                {
                    pair = new ValuePair() {Name = name, Value = value};
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