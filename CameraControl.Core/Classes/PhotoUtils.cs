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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Media;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using CameraControl.Devices;
using Eagle._Containers.Public;

#endregion

namespace CameraControl.Core.Classes
{
    public class PhotoUtils
    {
        public static bool RunAndWait(string exe, string param)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exe);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                startInfo.Arguments = param;
                Process process = Process.Start(startInfo);
                process.WaitForExit();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool Run(string exe)
        {
            return Run(exe, "", ProcessWindowStyle.Minimized) != null;
        }

        public static bool Run(string exe, string param)
        {
            return Run(exe, param, ProcessWindowStyle.Minimized) != null;
        }

        public static Process Run(string exe, string param, ProcessWindowStyle processWindowStyle)
        {
            Process process = null;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exe);
                startInfo.WindowStyle = processWindowStyle;
                if (!string.IsNullOrEmpty(param))
                    startInfo.Arguments = param;
                process = Process.Start(startInfo);
                //process.WaitForExit();
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return null;
            }
            return process;
        }

        [Obsolete("Not used anymore", false)]
        public static void CopyPhotoScale(string sourse, string dest, double scale)
        {
            File.Copy(sourse, dest, true);
        }


        public static void PlayCaptureSound()
        {
            try
            {
                string basedir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (basedir != null)
                {
                    var mplayer = new SoundPlayer(Path.Combine(basedir, "Data", "takephoto.wav"));
                    mplayer.Play();
                }
            }
            catch (Exception exception)
            {
                Log.Debug(exception);
            }
        }

        public static string GetFullPath(string path)
        {
            if (path.Contains(":\\"))
                return path;
            return Path.Combine(Settings.ApplicationFolder, path);
        }

        public static void Donate()
        {
            Run("http://www.digicamcontrol.com/donate/");
        }

        public static bool IsFileLocked(string file)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void WaitForFile(string file)
        {
            if (!File.Exists(file))
                return;
            int retry = 15;
            while (IsFileLocked(file) && retry > 0)
            {
                Thread.Sleep(100);
                retry--;
            }
        }

        public static string ReplaceExtension(string file, string ext)
        {
            return Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ext);
        }

        public static Boolean IsNumeric(Object expression)
        {
            if (expression == null || expression is DateTime)
                return false;

            if (expression is Int16 || expression is Int32 || expression is Int64 || expression is Decimal ||
                expression is Single || expression is Double || expression is Boolean)
                return true;

            try
            {
                if (expression is string)
                    Double.Parse(expression as string);
                else
                    Double.Parse(expression.ToString());
                return true;
            }
            catch
            {
            } // just dismiss errors but return false
            return false;
        }


        /// <summary>
        /// Create forlder for the specified filename
        /// </summary>
        /// <param name="Full path to filename"></param>
        public static void CreateFolder(string filename)
        {
            var folder = Path.GetDirectoryName(filename);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                //DirectorySecurity sec = Directory.GetAccessControl(folder);
                //// Using this instead of the "Everyone" string means we work on non-English systems.
                //SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                //sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                //Directory.SetAccessControl(folder, sec);
            }
        }

        public static string GetNextFileName(string file)
        {
            if (!File.Exists(file))
                return file;

            var ext = Path.GetExtension(file);
            var fileName = Path.GetFileNameWithoutExtension(file);
            var folder = Path.GetDirectoryName(file);
            int counter = 0;
            while (true)
            {
                var newfile = Path.Combine(folder, fileName+counter.ToString("0000") + ext);
                if (!File.Exists(newfile))
                    return newfile;
                counter++;
            }
        }

        public static int GetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }

        public static double Getdouble(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            return Convert.ToDouble(s, CultureInfo.InvariantCulture);
        }

    }
}