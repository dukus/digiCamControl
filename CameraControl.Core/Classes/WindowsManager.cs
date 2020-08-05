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
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using MaterialDesignThemes.Wpf;

#endregion

namespace CameraControl.Core.Classes
{
    public class WindowsManager
    {
        public AsyncObservableCollection<WindowCommandItem> WindowCommands { get; set; }


        public delegate void EventEventHandler(string cmd, object o);

        public virtual event EventEventHandler Event;

        private List<IWindow> WindowsList;

        public WindowsManager()
        {
            WindowsList = new List<IWindow>();
            WindowCommands = new AsyncObservableCollection<WindowCommandItem>();
        }

        public void Add(IWindow window)
        {
            WindowsList.Add(window);
        }

        public void ExecuteCommand(string cmd)
        {
            ExecuteCommand(cmd, null);
        }

        public void ExecuteCommand(string cmd, object o)
        {
            foreach (IWindow window in WindowsList)
            {
                try
                {
                    window.ExecuteCommand(cmd, o);
                }
                catch (Exception exception)
                {
                    Log.Error("Error to procces command " + cmd, exception);
                }

            }
            if (Event != null)
                Event(cmd, o);
        }

        public void ApplyTheme()
        {
            new PaletteHelper().ReplacePrimaryColor(ServiceProvider.Settings.CurrentThemeNameNew.Split('\\')[1]);
            new PaletteHelper().SetLightDark(ServiceProvider.Settings.CurrentThemeNameNew.StartsWith("Dark"));
        }

        public void ApplyKeyHanding()
        {
            foreach (Window win in WindowsList.OfType<Window>())
            {
                win.KeyDown += (sender, args) => TriggerClass.KeyDown(args);
            }
        }


        public IWindow Get(Type t)
        {
            return WindowsList.FirstOrDefault(window => window.GetType() == t);
        }

        public void Remove(string type)
        {
            IWindow windowToRemove = null;
            foreach (IWindow window in WindowsList.Where(window => window.GetType().ToString() == type))
            {
                windowToRemove = window;
            }
            if (windowToRemove != null)
                WindowsList.Remove(windowToRemove);
        }

        /// <summary>
        /// Registers commands used by the application core.
        /// </summary>
        public void RegisterKnowCommands()
        {
            AddCommandsFromType(typeof (CmdConsts));
            AddCommandsFromType(typeof (WindowsCmdConsts));
            WindowCommands = new AsyncObservableCollection<WindowCommandItem>(WindowCommands.OrderBy(x => x.Name));
            foreach (WindowCommandItem item in WindowCommands)
            {
                switch (item.Name)
                {
                    case CmdConsts.Capture:
                        item.SetKey(Key.Space);
                        break;
                    case WindowsCmdConsts.Next_Image:
                        item.SetKey(Key.Right);
                        break;
                    case WindowsCmdConsts.Prev_Image:
                        item.SetKey(Key.Left);
                        break;
                    case WindowsCmdConsts.Like_Image:
                        item.SetKey(Key.P);
                        break;
                    case WindowsCmdConsts.Unlike_Image:
                        item.SetKey(Key.X);
                        break;
                    case WindowsCmdConsts.SelectAll_Image:
                        item.SetKey(Key.A);
                        item.Ctrl = true;
                        break;
                    case CmdConsts.NextSeries:
                        item.SetKey(Key.Add);
                        break;
                    case CmdConsts.PrevSeries:
                        item.SetKey(Key.Subtract);
                        break;
                }
            }
        }

        private void AddCommandsFromType(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static |
                                        BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly).
                ToList();
            foreach (FieldInfo fieldInfo in fields)
            {
                WindowCommands.Add(new WindowCommandItem() {Name = fieldInfo.GetValue(type).ToString()});
            }
        }
    }
}