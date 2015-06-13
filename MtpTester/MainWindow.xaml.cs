using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Custom;
using CameraControl.Devices.TransferProtocol;
using CameraControl.Devices.Xml;
using Microsoft.Win32;
using PortableDeviceLib;

namespace MtpTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public PortableDevice SelectedDevice { get; set; }
        private BaseMTPCamera MTPCamera;
        private XmlDeviceData DeviceInfo;
        private XmlDeviceData DefaultDeviceInfo;

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("baseMtpDevice.xml"))
            {
                DefaultDeviceInfo = XmlDeviceData.Load("baseMtpDevice.xml");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeviceInfo = new XmlDeviceData();
            SelectDevice wnd = new SelectDevice();
            wnd.ShowDialog();
            if (wnd.DialogResult == true && wnd.SelectedDevice != null)
            {
                try
                {
                    SelectedDevice = wnd.SelectedDevice;
                    DeviceDescriptor descriptor = new DeviceDescriptor {WpdId = SelectedDevice.DeviceId};
                    MtpProtocol device = new MtpProtocol(descriptor.WpdId);
                    device.ConnectToDevice("MTPTester", 1, 0);
                    descriptor.StillImageDevice = device;
                    MTPCamera = new BaseMTPCamera();
                    MTPCamera.Init(descriptor);
                    LoadDeviceData(MTPCamera.ExecuteReadDataEx(0x1001));
                    //LoadDeviceData(MTPCamera.ExecuteReadDataEx(0x9108));
     
                    PopulateProperties();
                }
                catch (DeviceException exception)
                {
                    MessageBox.Show("Error getting device information" + exception.Message);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("General error" + exception.Message);
                }
                if (DefaultDeviceInfo != null)
                {
                    foreach (XmlCommandDescriptor command in DeviceInfo.AvaiableCommands)
                    {
                        command.Name = DefaultDeviceInfo.GetCommandName(command.Code);
                    }
                    foreach (XmlEventDescriptor avaiableEvent in DeviceInfo.AvaiableEvents)
                    {
                        avaiableEvent.Name = DefaultDeviceInfo.GetEventName(avaiableEvent.Code);
                    }
                    foreach (XmlPropertyDescriptor property in DeviceInfo.AvaiableProperties)
                    {
                        property.Name = DefaultDeviceInfo.GetPropName(property.Code);
                    }
                }
                InitUi();
            }

        }

        private void LoadDeviceData(MTPDataResponse res)
        {
            ErrorCodes.GetException(res.ErrorCode);
            DeviceInfo.Manufacturer = SelectedDevice.Manufacturer;
            DeviceInfo.Model = SelectedDevice.Model;
            int index = 2 + 4 + 2;
            int vendorDescCount = res.Data[index];
            index += vendorDescCount * 2;
            index += 3;
            int comandsCount = res.Data[index];
            index += 2;
            // load commands
            for (int i = 0; i < comandsCount; i++)
            {
                index += 2;
                DeviceInfo.AvaiableCommands.Add(new XmlCommandDescriptor() { Code = BitConverter.ToUInt16(res.Data, index) });
            }
            index += 2;
            int eventcount = res.Data[index];
            index += 2;
            // load events
            for (int i = 0; i < eventcount; i++)
            {
                index += 2;
                DeviceInfo.AvaiableEvents.Add(new XmlEventDescriptor() { Code = BitConverter.ToUInt16(res.Data, index) });
            }
            index += 2;
            int propertycount = res.Data[index];
            index += 2;
            // load properties codes
            for (int i = 0; i < propertycount; i++)
            {
                index += 2;
                DeviceInfo.AvaiableProperties.Add(new XmlPropertyDescriptor() { Code = BitConverter.ToUInt16(res.Data, index) });
            }
            MTPDataResponse vendor_res = MTPCamera.ExecuteReadDataEx(0x90CA);
            if (vendor_res.Data.Length > 0)
            {
                index = 0;
                propertycount = vendor_res.Data[index];
                index += 2;
                for (int i = 0; i < propertycount; i++)
                {
                    index += 2;
                    DeviceInfo.AvaiableProperties.Add(new XmlPropertyDescriptor() { Code = BitConverter.ToUInt16(vendor_res.Data, index) });
                }
            }   
        }


        private void PopulateProperties()
        {
            foreach (XmlPropertyDescriptor xmlPropertyDescriptor in DeviceInfo.AvaiableProperties)
            {
                try
                {
                    int index = 0;
                    MTPDataResponse result = MTPCamera.ExecuteReadDataEx(BaseMTPCamera.CONST_CMD_GetDevicePropDesc,
                                                                         xmlPropertyDescriptor.Code);
                    ErrorCodes.GetException(result.ErrorCode);
                    uint dataType = BitConverter.ToUInt16(result.Data, 2);
                    xmlPropertyDescriptor.DataType = dataType;
                    int dataLength = StaticHelper.GetDataLength(dataType);
                    if (dataLength < 1)
                        continue;
                    index += 4;
                    byte datareadonly = result.Data[index];
                    index += 1;
                    //factory def
                    index += dataLength;
                    // current value
                    index += dataLength;

                    byte formFlag = result.Data[index];
                    index += 1;
                    xmlPropertyDescriptor.DataForm = formFlag;
                    //UInt16 defval = BitConverter.ToUInt16(result.Data, 7);
                    if (formFlag == 2)
                    {
                        int length = BitConverter.ToInt16(result.Data, index);
                        index += 2;
                        for (int i = 0; i < length; i++)
                        {
                            long val = StaticHelper.GetValue(result, index, dataLength);
                            ;
                            xmlPropertyDescriptor.Values.Add(new XmlPropertyValue() {Value = val});
                            index += dataLength;
                        }
                    }
                    if (formFlag == 1)
                    {
                        xmlPropertyDescriptor.MinVal = StaticHelper.GetValue(result, index, dataLength);
                        index += dataLength;
                        xmlPropertyDescriptor.MaxVal = StaticHelper.GetValue(result, index, dataLength);
                        index += dataLength;
                        xmlPropertyDescriptor.Inc = StaticHelper.GetValue(result, index, dataLength);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error process property " + exception.Message);
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Xml file (*.xml)|*.xml";
            if (dialog.ShowDialog() == true)
            {
                Save(dialog.FileName);
            }
        }

        public void Save(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (XmlDeviceData));
                // Create a FileStream to write with.

                Stream writer = new FileStream(filename, FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, DeviceInfo);
                writer.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Unable to save data " + exception.Message);
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Xml file (*.xml)|*.xml";
                if (dialog.ShowDialog() == true)
                {
                    DeviceInfo = XmlDeviceData.Load(dialog.FileName);
                }
                InitUi();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error loading file " + exception.Message);
            }
        }


        private void InitUi()
        {
            lst_opers.ItemsSource = DeviceInfo.AvaiableCommands;
            lst_events.ItemsSource = DeviceInfo.AvaiableEvents;
            lst_prop.ItemsSource = DeviceInfo.AvaiableProperties;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MTPCamera == null || MTPCamera.IsConnected == false)
                {
                    DeviceInfo = new XmlDeviceData();
                    SelectDevice wnd = new SelectDevice();
                    wnd.ShowDialog();
                    if (wnd.DialogResult == true && wnd.SelectedDevice != null)
                    {
                        SelectedDevice = wnd.SelectedDevice;
                        DeviceDescriptor descriptor = new DeviceDescriptor {WpdId = SelectedDevice.DeviceId};
                        MTPCamera = new BaseMTPCamera();
                        MTPCamera.Init(descriptor);
                    }
                    else
                    {
                        return;
                    }
                }
                XmlPropertyValue property = lst_values.SelectedItem as XmlPropertyValue;
                XmlPropertyDescriptor propertyDescriptor = lst_prop.SelectedItem as XmlPropertyDescriptor;
                if (property != null)
                {
                    MTPCamera.SetProperty(BaseMTPCamera.CONST_CMD_SetDevicePropValue,
                                          BitConverter.GetBytes(property.Value), propertyDescriptor.Code);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error set property " + exception.Message);
            }
        }

        private void btn_get_value_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MTPCamera == null || MTPCamera.IsConnected == false)
                {
                    DeviceInfo = new XmlDeviceData();
                    SelectDevice wnd = new SelectDevice();
                    wnd.ShowDialog();
                    if (wnd.DialogResult == true && wnd.SelectedDevice != null)
                    {
                        SelectedDevice = wnd.SelectedDevice;
                        DeviceDescriptor descriptor = new DeviceDescriptor {WpdId = SelectedDevice.DeviceId};
                        MTPCamera = new BaseMTPCamera();
                        MTPCamera.Init(descriptor);
                    }
                    else
                    {
                        return;
                    }
                }
                XmlPropertyDescriptor propertyDescriptor = lst_prop.SelectedItem as XmlPropertyDescriptor;
                MTPDataResponse resp = MTPCamera.ExecuteReadDataEx(BaseMTPCamera.CONST_CMD_GetDevicePropValue,
                                                                    propertyDescriptor.Code);
                long val = StaticHelper.GetValue(resp, 0, StaticHelper.GetDataLength(propertyDescriptor.DataType));
                XmlPropertyValue selected = null;
                foreach (XmlPropertyValue xmlPropertyValue in propertyDescriptor.Values)
                {
                    if (xmlPropertyValue.Value == val)
                        selected = xmlPropertyValue;
                }
                if(selected!=null)
                {
                    lst_values.BeginInit();
                    lst_values.SelectedItem = selected;
                    lst_values.EndInit();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error to get value " + exception.Message);
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Xml file (*.xml)|*.xml";
            if (dialog.ShowDialog() == true)
            {
                SaveData(dialog.FileName);
            }
        }

        public void SaveData(string filename)
        {
            try
            {
                DeviceDescription description = new DeviceDescription();
                description.Model = DeviceInfo.Model;
                description.Manufacturer = DeviceInfo.Manufacturer;
                foreach (var property in DeviceInfo.AvaiableProperties)
                {
                    var prop = new DeviceProperty {Code = property.Code};
                    foreach (var propertyValue in property.Values)
                    {
                        prop.Values.Add(new DevicePropertyValue() {Value = propertyValue.Value});
                    }
                    description.Properties.Add(prop);
                }

                XmlSerializer serializer = new XmlSerializer(typeof (DeviceDescription));
                // Create a FileStream to write with.

                Stream writer = new FileStream(filename, FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, description);
                writer.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Unable to save data " + exception.Message);
            }
        }

        //private void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    using (StreamReader sr = new StreamReader("d:\\events.txt"))
        //    {
        //        String line;
        //        // Read and display lines from the file until the end of
        //        // the file is reached.
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            line = line.Trim();
        //            string name = line.Substring(8, line.Length - 8 - 6).Trim();
        //            string codestr = line.Substring(line.Length - 6);
        //            if (codestr.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        //                codestr = codestr.Substring(2);
        //            int code = int.Parse(codestr, NumberStyles.HexNumber);
        //            DeviceInfo.AvaiableEvents.Add(new XmlEventDescriptor(){Code = (uint) code,Name = name});
        //        }
        //    }
        //}
    }
}
