using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

using CameraControl.Devices;

namespace MtpTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += AppDispatcherUnhandledException;
        }




        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //#if DEBUG
            //      // In debug mode do not custom-handle the exception, let Visual Studio handle it

            //      e.Handled = false;

            //#else

            //          ShowUnhandeledException(e);    

            //#endif
            ShowUnhandeledException(e);
        }

        private void ShowUnhandeledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            Log.Error("Unhandled error ", e.Exception);

            string errorMessage =
                string.Format(
                    "Unhandled error {0}",

                    e.Exception.Message + (e.Exception.InnerException != null
                                               ? "\n" +
                                                 e.Exception.InnerException.Message
                                               : null));

            if (
                MessageBox.Show(errorMessage, "Unhandled error", MessageBoxButton.YesNo,
                                MessageBoxImage.Error) ==
                MessageBoxResult.No)
            {
                if (Current != null)
                    Current.Shutdown();
            }

        }
    }
}
