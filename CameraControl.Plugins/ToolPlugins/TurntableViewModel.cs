using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Devices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Model.StepMotor;
using Photo3D.StepMotor;

namespace CameraControl.Plugins.ToolPlugins
{
    public class TurntableViewModel : ViewModelBase
    {

        VRStepMotorAPI api = new VRStepMotorAPI();
        private bool _isConnected;
        private string _status;

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand RotateCommand { get; set; }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                RaisePropertyChanged(()=>IsConnected);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }


        public TurntableViewModel()
        {
            ConnectCommand = new RelayCommand(Connect);
            RotateCommand = new RelayCommand(Rotate);
            IsConnected = false;
        }

        public void Connect()
        {
            try
            {
                MotorPortStatus status = api.Connect();
                IsConnected = false;
                switch (status)
                {
                    case MotorPortStatus.Disconnected:
                        Status = "Disconnected";
                        break;
                    case MotorPortStatus.Ready:
                        IsConnected = true;
                        Status = "Ready";
                        break;
                    case MotorPortStatus.Busy:
                        Status = "Busy";
                        break;

                }
            }
            catch (Exception e)
            {
                Log.Error("Error conect", e);
            }
        }

        public void Rotate()
        {
            try
            {
                api.Rotate(new Model.StepMotor.RotationParameters()
                {
                    NSteps = 10000,
                    RotationSpeed = 20,
                });
            }
            catch (Exception e)
            {
                Log.Error("Error rotate", e);
            }
        }
    }
}
