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
        private Dictionary<string, string> _replaceValues;
        private AsyncObservableCollection<T> _numericValues = new AsyncObservableCollection<T>();
        private AsyncObservableCollection<string> _values = new AsyncObservableCollection<string>();
        private bool _notifyValuChange = true;
        private readonly object _syncRoot = new object();
        public string Tag { get; set; }


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
        /// <summary>
        /// Indicate if the property is available and can be used 
        /// </summary>
        public bool Available
        {
            get { return _available; }
            set
            {
                _available = value;
                NotifyPropertyChanged("Available");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the propertie 
        /// settings wasn't errorless.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [have error]; otherwise, <c>false</c>.
        /// </value>
        public bool HaveError
        {
            get { return _haveError; }
            set
            {
                _haveError = value;
                NotifyPropertyChanged("HaveError");
                NotifyPropertyChanged("ErrorColor");
            }
        }

        public string ErrorColor
        {
            get { return HaveError ? "Red" : "Transparent"; }
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
            get
            {
                if (_value != null && _replaceValues != null && _replaceValues.Count > 0)
                {
                    foreach (var replaceValue in _replaceValues.Where(replaceValue => replaceValue.Value == _value))
                    {
                        return replaceValue.Key;
                    }
                }
                return _value;
            }
            set
            {
                _value = value;
                if (_value != null)
                {
                    if (_replaceValues != null && _replaceValues.Count > 0 && _replaceValues.ContainsKey(_value))
                    {
                        _value = _replaceValues[_value];
                    }

                    HaveError = false;
                    if (ValueChanged != null && _notifyValuChange)
                    {
                        if (_valuesDictionary.ContainsKey(_value))
                        {
                            OnValueChanged(this, _value, _valuesDictionary[_value]);
                        }
                    }
                }
                else
                {

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
            thread.Start(new object[] { sender, key, val });
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
                        T val = (T)objparams[2];
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
        private bool _haveError;
        private bool _available;

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
            get
            {
                //if (_valuesDictionary.Count > 0 && _values.Count == 0)
                //    ReloadValues();
                return _values;
            }
        }

        public AsyncObservableCollection<T> NumericValues
        {
            get { return _numericValues; }
        }

        public PropertyValue()
        {
            _valuesDictionary = new Dictionary<string, T>();
            _replaceValues = new Dictionary<string, string>();
            DisableIfWrongValue = false;
            IsEnabled = true;
            Available = true;
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

        public void Filter(List<T> values)
        {
            var old = new Dictionary<string, T>(_valuesDictionary);
            _valuesDictionary.Clear();
            foreach (var val in old)
            {
                if (values.Contains(val.Value))
                    _valuesDictionary.Add(val.Key, val.Value);
            }
            _values = new AsyncObservableCollection<string>(_valuesDictionary.Keys);
            _numericValues = new AsyncObservableCollection<T>(_valuesDictionary.Values);
            NotifyPropertyChanged("Values");
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
            if (typeof(T) == typeof(int))
            {
                int val = ba.Length == 1 ? ba[0] : BitConverter.ToInt16(ba, 0);
                SetValue((T)((object)val));
            }
            if (typeof(T) == typeof(uint))
            {
                uint val = BitConverter.ToUInt16(ba, 0);
                SetValue((T)((object)val));
            }
            if (typeof(T) == typeof(long) && SubType == typeof(sbyte))
            {
                long val = Convert.ToSByte(ba[0]);
                SetValue((T)((object)val));
                return;
            }
            if (typeof(T) == typeof(long) && ba.Length == 1)
            {
                long val = ba[0];
                SetValue((T)((object)val));
                return;
            }
            if (typeof(T) == typeof(long) && ba.Length == 2 && SubType == typeof(UInt16))
            {
                long val = BitConverter.ToUInt16(ba, 0);
                SetValue((T)((object)val));
                return;
            }
            if (typeof(T) == typeof(long) && ba.Length == 2)
            {
                long val = BitConverter.ToInt16(ba, 0);
                SetValue((T)((object)val));
                return;
            }
            if (typeof(T) == typeof(long) && ba.Length == 4)
            {
                long val = BitConverter.ToUInt32(ba, 0);
                SetValue((T)((object)val));
                return;
            }
            if (typeof(T) == typeof(long))
            {
                long val = BitConverter.ToInt64(ba, 0);
                SetValue((T)((object)val));
            }
            if (typeof(T) == typeof(uint))
            {
                uint val = BitConverter.ToUInt16(ba, 0);
                SetValue((T)((object)val));
            }
        }

        /// <summary>
        /// After value add finished, ReloadValues should be called
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValues(string key, T value)
        {

            if (!_valuesDictionary.ContainsKey(key))
                _valuesDictionary.Add(key, value);
        }

        /// <summary>
        /// Reload display values 
        /// Should be called always after AddValues
        /// </summary>
        public void ReloadValues()
        {
            _values = new AsyncObservableCollection<string>(_valuesDictionary.Keys);
            _numericValues = new AsyncObservableCollection<T>(_valuesDictionary.Values);
            NotifyPropertyChanged("Values");
        }

        /// <summary>
        /// Refresh display values based on replace values 
        /// </summary>
        public void GetValues()
        {
            lock (_syncRoot)
            {
                string newVal = Value;
                _values.Clear();
                NotifyPropertyChanged("Values");
                _values = new AsyncObservableCollection<string>(_valuesDictionary.Keys);
                if (_replaceValues != null && _replaceValues.Count > 0)
                {
                    for (int i = 0; i < _values.Count; i++)
                    {
                        if (_replaceValues.ContainsValue(_values[i]))
                        {
                            foreach (var value in _replaceValues)
                            {
                                if (value.Value == _values[i])
                                    _values[i] = value.Key;
                            }
                        }
                    }
                }
                _numericValues = new AsyncObservableCollection<T>(_valuesDictionary.Values);
                NotifyPropertyChanged("Values");
                Value = newVal;
            }
        }

        /// <summary>
        /// Replace a displayed value 
        /// GetValues() call is required
        /// </summary>
        /// <param name="oldVal"></param>
        /// <param name="newVal"></param>
        public void ReplaceValue(string oldVal, string newVal)
        {
            lock (_syncRoot)
            {
                if (!_replaceValues.ContainsKey(newVal))
                {
                    _replaceValues.Add(newVal, oldVal);
                }
            }
        }


        public void Clear(bool notify = true)
        {
            _valuesDictionary.Clear();
            _values.Clear();
            _numericValues.Clear();
            if (notify)
                NotifyPropertyChanged("Values");
        }
    }
}