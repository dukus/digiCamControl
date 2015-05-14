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
using System.Windows;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using MahApps.Metro.Controls.Dialogs;

#endregion

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for EditSession.xaml
    /// </summary>
    public partial class EditSession
    {
        public PhotoSession Session { get; set; }

        public EditSession(PhotoSession session)
        {
            try
            {
                Session = session;
                Session.BeginEdit();
                InitializeComponent();
                DataContext = Session;
                ServiceProvider.Settings.ApplyTheme(this);
            }
            catch (Exception ex)
            {
                Log.Error("EditSession init", ex);
            }
        }

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.SelectedPath = Session.Folder;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Session.Folder = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync(TranslationStrings.LabelError, TranslationStrings.LabelErrorSetFolder);
                Log.Error("Error set folder ", ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Session.EndEdit();
            DialogResult = true;
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Session.CancelEdit();
            Close();
        }

        private void btn_add_tag_Click(object sender, RoutedEventArgs e)
        {
            TagItem item = new TagItem();
            EditTagWnd wnd = new EditTagWnd(item);
            if (wnd.ShowDialog() == true)
            {
                Session.Tags.Add(item);
            }
        }

        private void btn_del_tag_Click(object sender, RoutedEventArgs e)
        {
            TagItem item = lst_tags.SelectedItem as TagItem;
            if (item != null)
            {
                Session.Tags.Remove(item);
            }
        }

        private void btn_edit_tag_Click(object sender, RoutedEventArgs e)
        {
            TagItem item = lst_tags.SelectedItem as TagItem;
            if (item != null)
            {
                var wnd = new EditTagWnd(item);
                wnd.ShowDialog();
            }
        }

        private void btn_help_Click(object sender, RoutedEventArgs e)
        {
            HelpProvider.Run(HelpSections.Session);
        }

        private void btn_browse_bk_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = Session.BackUpPath;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Session.BackUpPath = dialog.SelectedPath;
            }
        }

        private void btn_template_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new FileNameTemplateEditorWnd();
            wnd.Owner = this;
            wnd.TemplateString = Session.FileNameTemplate;
            if (wnd.ShowDialog() == true)
            {
                Session.FileNameTemplate = wnd.TemplateString;
            }
        }

    }
}