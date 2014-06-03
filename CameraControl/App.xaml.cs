using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using CameraControl.Actions;
using CameraControl.Actions.Enfuse;
using CameraControl.Classes;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Translation;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.windows;
using Application = System.Windows.Application;
using HelpProvider = CameraControl.Classes.HelpProvider;
using MessageBox = System.Windows.MessageBox;

namespace CameraControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Global exception handling  
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
                TranslationStrings.LabelUnHandledError,

                e.Exception.Message + (e.Exception.InnerException != null
                                         ? "\n" +
                                           e.Exception.InnerException.Message
                                         : null));
            // check if wia 2.0 is registered 
            // isn't a clean way
            if (errorMessage.Contains("{E1C5D730-7E97-4D8A-9E42-BBAE87C2059F}"))
            {
                System.Windows.Forms.MessageBox.Show(TranslationStrings.LabelWiaNotInstalled);
                PhotoUtils.RunAndWait("regwia.bat", "");
                System.Windows.Forms.MessageBox.Show(TranslationStrings.LabelRestartTheApplication);
                Application.Current.Shutdown();
            }
            else if (e.Exception.GetType() == typeof(OutOfMemoryException))
            {
                Log.Error("Out of memory. Application exiting ");
                System.Windows.Forms.MessageBox.Show(TranslationStrings.LabelOutOfMemory);
                if (Current != null)
                    Current.Shutdown();
            }
            else
            {
                if (MessageBox.Show(TranslationStrings.LabelAskSendLogFile, TranslationStrings.LabelApplicationError, MessageBoxButton.YesNo,
                        MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    var wnd = new ErrorReportWnd("Application crash " + e.Exception.Message, e.Exception.StackTrace);
                    wnd.ShowDialog();
                }
                if (
                MessageBox.Show(errorMessage, TranslationStrings.LabelApplicationError, MessageBoxButton.YesNo,
                                MessageBoxImage.Error) ==
                MessageBoxResult.No)
                {
                    if (Current != null)
                        Current.Shutdown();
                }
            }
        }

    }
}
