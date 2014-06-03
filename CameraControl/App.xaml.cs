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

#endregion

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
            else if (e.Exception.GetType() == typeof (OutOfMemoryException))
            {
                Log.Error("Out of memory. Application exiting ");
                System.Windows.Forms.MessageBox.Show(TranslationStrings.LabelOutOfMemory);
                if (Current != null)
                    Current.Shutdown();
            }
            else
            {
                if (MessageBox.Show(TranslationStrings.LabelAskSendLogFile, TranslationStrings.LabelApplicationError,
                                    MessageBoxButton.YesNo,
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