using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;

namespace CameraControl.Service
{
    public class CameraWindowsService : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public CameraWindowsService()
        {
            // Name the Windows Service
            ServiceName = "CameraWindowsService";
        }

        public static void Main()
        {
            ServiceBase.Run(new CameraWindowsService());
        }

        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(CameraService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }




    }
}
