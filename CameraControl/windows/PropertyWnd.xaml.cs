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
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using MessageBox = System.Windows.Forms.MessageBox;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for PropertyWnd.xaml
    /// </summary>
    public partial class PropertyWnd
    {
        public PropertyWnd()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
                                                   new ExecutedRoutedEventHandler(delegate { this.Close(); })));
            ServiceProvider.Settings.ApplyTheme(this);
        }

        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsVisible)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            btn_set.IsEnabled = false;
            try
            {
                string filename = ServiceProvider.Settings.SelectedBitmap.FileItem.FileName;
                Exiv2Helper.SaveComment(filename, ServiceProvider.Settings.SelectedBitmap.Comment);
                if (
                    ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.ExifTags.ContainName(
                        "Iptc.Application2.Caption"))
                    ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.ExifTags["Iptc.Application2.Caption"] =
                        ServiceProvider.Settings.SelectedBitmap.Comment;
                else
                {
                    ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.ExifTags.Add(new ValuePair()
                                                                                               {
                                                                                                   Name =
                                                                                                       "Iptc.Application2.Caption",
                                                                                                   Value =
                                                                                                       ServiceProvider.
                                                                                                       Settings.
                                                                                                       SelectedBitmap.
                                                                                                       Comment
                                                                                               });
                }
                ServiceProvider.Settings.SelectedBitmap.FileItem.SaveInfo();
                if (chk_tags.IsChecked == true)
                {
                    Exiv2Helper.DelKeyword(filename);
                    if (!string.IsNullOrEmpty(ServiceProvider.Settings.DefaultSession.SelectedTag1.Value))
                        Exiv2Helper.AddKeyword(filename, ServiceProvider.Settings.DefaultSession.SelectedTag1.Value);
                    if (!string.IsNullOrEmpty(ServiceProvider.Settings.DefaultSession.SelectedTag2.Value))
                        Exiv2Helper.AddKeyword(filename, ServiceProvider.Settings.DefaultSession.SelectedTag2.Value);
                    if (!string.IsNullOrEmpty(ServiceProvider.Settings.DefaultSession.SelectedTag3.Value))
                        Exiv2Helper.AddKeyword(filename, ServiceProvider.Settings.DefaultSession.SelectedTag3.Value);
                    if (!string.IsNullOrEmpty(ServiceProvider.Settings.DefaultSession.SelectedTag4.Value))
                        Exiv2Helper.AddKeyword(filename, ServiceProvider.Settings.DefaultSession.SelectedTag4.Value);
                }
                if (Path.GetFileNameWithoutExtension(filename) != ServiceProvider.Settings.SelectedBitmap.FileName)
                {
                    try
                    {
                        string newfilename = Path.Combine(Path.GetDirectoryName(filename),
                                                          ServiceProvider.Settings.SelectedBitmap.FileName +
                                                          Path.GetExtension(filename));
                        File.Copy(filename, newfilename);
                        File.Delete(filename);
                        Thread.Sleep(200);
                        int i =
                            ServiceProvider.Settings.DefaultSession.Files.IndexOf(
                                ServiceProvider.Settings.SelectedBitmap.FileItem);
                        FileItem item = new FileItem(newfilename);
                        ServiceProvider.Settings.DefaultSession.Files.Remove(
                            ServiceProvider.Settings.SelectedBitmap.FileItem);
                        ServiceProvider.Settings.SelectedBitmap.FileItem.RemoveThumbs();
                        ServiceProvider.Settings.DefaultSession.Files.Insert(i, item);
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Select_Image, item);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Error rename file" + exception.Message);
                        Log.Error("Error rename file", exception);
                    }
                }
                btn_set.IsEnabled = true;
            }
            catch (Exception exception)
            {
                Log.Error("Error set property ", exception);
                MessageBox.Show("Error set property !" + exception.Message);
            }
        }
    }
}