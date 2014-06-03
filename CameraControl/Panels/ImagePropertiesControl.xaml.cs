using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;

namespace CameraControl.Panels
{
    /// <summary>
    /// Interaction logic for ImagePropertiesControl.xaml
    /// </summary>
    public partial class ImagePropertiesControl : UserControl
    {
        public ImagePropertiesControl()
        {
            InitializeComponent();
        }

        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            btn_set.IsEnabled = false;
            try
            {

                string filename = ServiceProvider.Settings.SelectedBitmap.FileItem.FileName;
                Exiv2Helper.SaveComment(filename, ServiceProvider.Settings.SelectedBitmap.Comment);
                if (ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.ExifTags.ContainName("Iptc.Application2.Caption"))
                    ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.ExifTags["Iptc.Application2.Caption"] =
                        ServiceProvider.Settings.SelectedBitmap.Comment;
                else
                {
                    ServiceProvider.Settings.SelectedBitmap.FileItem.FileInfo.ExifTags.Add(new ValuePair()
                    {
                        Name =
                            "Iptc.Application2.Caption",
                        Value =
                            ServiceProvider.Settings.
                            SelectedBitmap.Comment
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
                if (System.IO.Path.GetFileNameWithoutExtension(filename) != ServiceProvider.Settings.SelectedBitmap.FileName)
                {
                    try
                    {
                        string newfilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename),
                                                          ServiceProvider.Settings.SelectedBitmap.FileName +
                                                          Path.GetExtension(filename));
                        File.Copy(filename, newfilename);
                        File.Delete(filename);
                        Thread.Sleep(200);
                        int i =
                            ServiceProvider.Settings.DefaultSession.Files.IndexOf(
                                ServiceProvider.Settings.SelectedBitmap.FileItem);
                        FileItem item = new FileItem(newfilename);
                        ServiceProvider.Settings.DefaultSession.Files.Remove(ServiceProvider.Settings.SelectedBitmap.FileItem);
                        ServiceProvider.Settings.SelectedBitmap.FileItem.RemoveThumbs();
                        ServiceProvider.Settings.DefaultSession.Files.Insert(i, item);
                        ServiceProvider.WindowsManager.ExecuteCommand(WindowsCmdConsts.Select_Image, item);
                    }
                    catch (Exception exception)
                    {
                        System.Windows.Forms.MessageBox.Show("Error rename file" + exception.Message);
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
