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
using System.Threading;
using PortableDeviceLib;

#endregion

namespace CameraControl.Devices.Classes
{
    public class PropertyValue<T> : BaseFieldClass
    {
        public delegate void ValueChangedEventHandler(object sender, string key, T val);

        public event ValueChangedEventHandler ValueChanged;

        private Dictionary<string, T> _valuesDictionary;
        private AsyncObservableCollection<T> _numericValues = new AsyncObservableCollection<T>();
        private AsyncObservableCollection<string> _values = new AsyncObservableCollection<string>();
        private bool _notifyValuChange = true;
        private readonly object _syncRoot = new object();


        private uint _code;

        public uint Code
        {
            get { return _code; }
            set
            {
                _code = value;
                NotifyPropertyChanged("Code");
            }
        }

        public Type SubType { get; set; }
        public bool DisableIfWrongValue { get; set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                lock (_syncRoot)
                {
                    _value = value;
                    if (ValueChanged != null && _notifyValuChange)
                    {
                        foreach (KeyValuePair<string, T> keyValuePair in _valuesDictionary)
                        {
                            if (keyValuePair.Key == _value)
                            {
                                OnValueChanged(this, _value, keyValuePair.Value);
                            }
                        }
                    }
                }
                NotifyPropertyChanged("Value");
            }
        }

        private T _numericValue;

        public T NumericValue
        {
            get { return _numericValue; }
            set
            {
                _numericValue = value;
                NotifyPropertyChanged("NumericValue");
            }
        }

        public void NextValue()
        {
            lock (_syncRoot)
            {
                if (Values == null || Values.Count == 0 || !IsEnabled)
                    return;
                int ind = Values.IndexOf(Value);
                if (ind < 0)
                    return;
                ind++;
                if (ind < Values.Count)
                    Value = Values[ind];
            }
        }

        public void PrevValue()
        {
            lock (_syncRoot)
            {
                if (Values == null || Values.Count == 0 || !IsEnabled)
                    return;
                int ind = Values.IndexOf(Value);
                ind--;
                if (ind < 0)
                    return;

                if (ind < Values.Count)
                    Value = Values[ind];
            }
        }


        public void OnValueChanged(object sender, string key, T val)
        {
            Thread thread = new Thread(OnValueChangedThread);
            thread.Name = "SetProperty thread " + Name;
            thread.Start(new object[] {sender, key, val});
            thread.Join(200);
        }

        public void OnValueChangedThread(object obj)
        {
            lock (_syncRoot)
            {
                object[] objparams = obj as object[];
                bool retry;
                int retrynum = 5;
                do
                {
                    retry = false;
                    try
                    {
                        object sender = objparams[0];
                        string key = objparams[1] as string;
                        T val = (T) objparams[2];
                        ValueChanged(sender, key, val);
                    }
                    catch (DeviceException exception)
                    {
                        if ((exception.ErrorCode == ErrorCodes.ERROR_BUSY ||
                             exception.ErrorCode == ErrorCodes.MTP_Device_Busy) && retrynum > 0)
                        {
                            retrynum--;
                            retry = true;
                            Thread.Sleep(100);
                        }
                    }
                } while (retry);
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get
            {
                //if (Values == null || Values.Count==0)
                //  return false;
                //if (DisableIfWrongValue)
                //{
                //    return _isEnabled && !string.IsNullOrEmpty(Value);
                //}
                //else
                //{
                //    return _isEnabled;
                //}
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged("IsEnabled");
            }
        }

        public AsyncObservableCollection<string> Values
        {
            get { return _values; }
        }

        public AsyncObservableCollection<T> NumericValues
        {
            get { return _numericValues; }
        }

        public PropertyValue()
        {
            _valuesDictionary = new Dictionary<string, T>();
            DisableIfWrongValue = false;
            IsEnabled = true;
        }

        public void SetValue(T o, bool notifyValuChange)
        {
            _notifyValuChange = notifyValuChange;
            SetValue(o);
            _notifyValuChange = true;
        }

        public void SetValue(T o)
        {
            lock (_syncRoot)
            {
                NumericValue = o;
                foreach (KeyValuePair<string, T> keyValuePair in _valuesDictionary)
                {
                    if (EqualityComparer<T>.Default.Equals(keyValuePair.Value, o))
                        //(keyValuePair.Value== o)
                    {
                        Value = keyValuePair.Key;
                        return;
                    }
                }
            }
        }
    

        public void SetValue()
        {
            Value = Value;
        }


        public void SetValue(string o, bool notifyValuChange = true)
        {
            lock (_syncRoot)
            {
                foreach (KeyValuePair<string, T> keyValuePair in _valuesDictionary)
                {
                    if (keyValuePair.Key == o)
                    {
                        _notifyValuChange = notifyValuChange;
                        Value = keyValuePair.Key;
                        _notifyValuChange = true;
                        return;
                    }
                }
            }
        }

        public void SetValue(byte[] ba, bool notifyValuChange)
        {
            _notifyValuChange = notifyValuChange;
            SetValue(ba);
            _notifyValuChange = true;
        }

        public void SetValue(MTPDataResponse data, bool notifyValuChange)
        {
            _notifyValuChange = notifyValuChange;
            SetValue(data.Data);
            _notifyValuChange = true;
        }

        public void SetValue(byte[] ba)
        {
            if (ba == null || ba.Length < 1)
                return;
            if (typeof (T) == typeof (int))
            {
                int val = ba.Length == 1 ? ba[0] : BitConverter.ToInt16(ba, 0);
                SetValue((T) ((object) val));
            }
            if (typeof (T) == typeof (uint))
            {
                uint val = BitConverter.ToUInt16(ba, 0);
                SetValue((T) ((object) val));
            }
            if (typeof (T) == typeof (long) && ba.Length == 1)
            {
                long val = ba[0];
                SetValue((T) ((object) val));
                return;
            }
            if (typeof (T) == typeof (long) && ba.Length == 2 && SubType == typeof (UInt16))
            {
                long val = BitConverter.ToUInt16(ba, 0);
                SetValue((T) ((object) val));
                return;
            }
            if (typeof (T) == typeof (long) && ba.Length == 2)
            {
                long val = BitConverter.ToInt16(ba, 0);
                SetValue((T) ((object) val));
                return;
            }
            if (typeof (T) == typeof (long) && ba.Length == 4)
            {
                long val = BitConverter.ToInt32(ba, 0);
                SetValue((T) ((object) val));
                return;
            }
            if (typeof (T) == typeof (long))
            {
                long val = BitConverter.ToInt32(ba, 0);
                SetValue((T) ((object) val));
            }
            if (typeof (T) == typeof (uint))
            {
                uint val = BitConverter.ToUInt16(ba, 0);
                SetValue((T) ((object) val));
            }
        }

        public void AddValues(string key, T value)
        {
            lock (_syncRoot)
            {
                if (!_valuesDictionary.ContainsKey(key))
                    _valuesDictionary.Add(key, value);
                _values = new AsyncObservableCollection<string>(_valuesDictionary.Keys);
                _numericValues = new AsyncObservableCollection<T>(_valuesDictionary.Values);
                NotifyPropertyChanged("Values");
            }
        }

        public void Clear()
        {
            _valuesDictionary.Clear();
        }
    }
}