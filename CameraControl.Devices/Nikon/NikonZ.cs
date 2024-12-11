using CameraControl.Devices.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Devices.Nikon
{
    public class NikonZ : NikonD500
    {
        protected override void InitFocusMode()
        {
            try
            {
                Log.Debug("InitFocusMode 1");
                DeviceReady();
                NormalFocusMode = new PropertyValue<long>();
                NormalFocusMode.Name = "FocusMode";
                NormalFocusMode.Code = CONST_PROP_AFModeSelect;
                NormalFocusMode.IsEnabled = true;
                NormalFocusMode.AddValues("MF", 0);
                NormalFocusMode.AddValues("AF-S", 0x8010);
                NormalFocusMode.AddValues("AF-C", 0x8011);
                NormalFocusMode.AddValues("AF-F", 0x8013);
                NormalFocusMode.ReloadValues();
                NormalFocusMode.ValueChanged += NormalFocusMode_ValueChanged;
                FocusMode = NormalFocusMode;
                FocusMode.IsEnabled = false; //FocusMode in NikonZ is supported only in LiveView
                ReadDeviceProperties(NormalFocusMode.Code);
                Log.Debug("InitFocusMode 2");
                LiveViewFocusMode = new PropertyValue<long>();
                LiveViewFocusMode.Name = "FocusMode";
                LiveViewFocusMode.Code = CONST_PROP_AfModeAtLiveView;
                LiveViewFocusMode.IsEnabled = true;
                LiveViewFocusMode.AddValues("AF-S", 0);
                LiveViewFocusMode.AddValues("AF-C", 1);
                LiveViewFocusMode.AddValues("MF", 4);
                LiveViewFocusMode.ReloadValues();
                LiveViewFocusMode.ValueChanged += LiveViewFocusMode_ValueChanged;
                ReadDeviceProperties(LiveViewFocusMode.Code);
                Log.Debug("InitFocusMode 3");
            }
            catch (Exception exception)
            {
                Log.Error("Unable to init focus mode property", exception);
            }
        }

        void LiveViewFocusMode_ValueChanged(object sender, string key, long val)
        {
            // CONST_PROP_AfModeAtLiveView accepts 1 single 8 bit value
            byte[] val8bit = new byte[1];
            val8bit[0] = (byte)val;

            SetProperty(CONST_CMD_SetDevicePropValue, val8bit,CONST_PROP_AfModeAtLiveView);
        }

        private void NormalFocusMode_ValueChanged(object sender, string key, long val)
        {
            SetProperty(CONST_CMD_SetDevicePropValue, BitConverter.GetBytes((sbyte)val),
                        CONST_PROP_AFModeSelect);
        }

    }
}