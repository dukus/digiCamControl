using CameraControl.Core;
using CameraControl.Devices;
using CameraControl.Devices.GoPro;
using CameraControl.Devices.Wifi;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;


namespace CameraControl.ViewModel
{
    public class BlueToothViewModel : ViewModelBase
    {
        private GDeviceInformation selectedItem;


        public GoProBluetoothHelper GoProBluetoothHelper { get; set; }
        public RelayCommand ReScanCommand { get; set; }
        public RelayCommand PairCommand { get; set; }
        public RelayCommand ConnectCommand { get; set; }
        public bool IsSelected { get; set; }
        public bool IsNotBusy { get; set; }

        public Window Window { get; set; }

        public GDeviceInformation SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                IsSelected = selectedItem != null;
                RaisePropertyChanged(() => IsSelected);
            }
        }


        public BlueToothViewModel()
        {
            IsNotBusy = true;
            ReScanCommand = new RelayCommand(ReScan);
            ConnectCommand = new RelayCommand(Connect);
            PairCommand = new RelayCommand(Pair);
            GoProBluetoothHelper = new GoProBluetoothHelper();
            if (!IsInDesignMode)
            {
                GoProBluetoothHelper.Initialize();
            }
        }

        private void Pair()
        {
            try
            {
                GoProBluetoothHelper.Pair(selectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error to connect to the device " + ex.Message);
            }
        }

        private void Connect()
        {
            IsNotBusy = false;
            RaisePropertyChanged(() => IsNotBusy);
            Task.Factory.StartNew(ConnectAsync);
        }
        private async void ConnectAsync()
        {
            BluetoothLEDevice mBLED = null;
            try
            {
                Log.Debug("Start bluetooth connection to: " + selectedItem.DisplayName);
                mBLED = BluetoothLEDevice.FromIdAsync(selectedItem.DeviceInfo.Id).AsTask().Result;
                
                if (!mBLED.DeviceInformation.Pairing.IsPaired)
                {
                    MessageBox.Show("Device not paired");
                    Log.Debug("Device not paired");
                    IsNotBusy = true;
                    RaisePropertyChanged(() => IsNotBusy);
                    return;
                }
                var device = new GoProBluetoothDevice(mBLED);
                Log.Debug("Enabling wifi");
                device.ExecuteCommand(GoProBluetoothDevice.BLE_CMD_WIFI_ON);
                Thread.Sleep(500);
                await device.ConnectWifiAsync();
                GoProProvider goProProvider = new GoProProvider();
                var deviceDescriptor = goProProvider.Connect(goProProvider.DefaultIp, device);
                ServiceProvider.DeviceManager.AddDevice(deviceDescriptor);
                App.Current.Dispatcher.Invoke(() => this.Window.Close());
            }
            catch (Exception ex)
            {
                Log.Error("Error to connect to the bluetooth device", ex);
                MessageBox.Show("Error to connect to the device " + ex.Message);
                if (mBLED != null)
                    mBLED.Dispose();
            }
            //App.Current.Dispatcher.Invoke(() => this.Window.Close());
        }

        private void ReScan()
        {
            GoProBluetoothHelper.Rescan();
        }
    }
}
