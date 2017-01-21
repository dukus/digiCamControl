using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CameraControl.Devices.Classes
{
    public static class HttpHelper
    {
        public static void DownLoadFileByWebRequest(string urlAddress, string filePath, ICameraDevice device,long size=0)
        {
            try
            {
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                request = (HttpWebRequest)WebRequest.Create(urlAddress);
                request.Timeout = 30000;  //8000 Not work 
                response = (HttpWebResponse)request.GetResponse();
                Stream s = response.GetResponseStream();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                FileStream os = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[102400];
                int c = 0;
                int totalsize = 0;
                while ((c = s.Read(buff, 0, 102400)) > 0)
                {
                    os.Write(buff, 0, c);
                    os.Flush();
                    totalsize += c;
                    if (size > 0)
                    {
                        device.TransferProgress = (uint)(totalsize/(double)size*100.0);
                    }
                    else
                    {
                        device.TransferProgress += 1;
                    }
                    
                }
                os.Close();
                s.Close();
                device.TransferProgress = 100;
            }
            catch(Exception ex)
            {
                Log.Error("Error download file", ex);
            }
            finally
            {
            }
        }
    }
}
