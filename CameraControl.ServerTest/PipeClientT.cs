using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Windows;

namespace CameraControl.ServerTest
{
    class PipeClientT
    {
        public string Send(string sendStr, string pipeName, int timeOut = 1000)
        {
            try
            {
                NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);

                pipeStream.Connect(timeOut);


                var sr = new StreamReader(pipeStream);
                var sw = new StreamWriter(pipeStream);
                 

                sw.WriteLine(sendStr);
                sw.Flush();

                string temp = sr.ReadToEnd();
                return temp;
            }
            catch (Exception oEX)
            {
                MessageBox.Show("Error :" + oEX.Message);
                Debug.WriteLine(oEX.Message);
            }
            return "";
        }

        private void AsyncSend(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeClientStream pipeStream = (NamedPipeClientStream)iar.AsyncState;

                // End the write
                pipeStream.EndWrite(iar);
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();
            }
            catch (Exception oEx)
            {
                Debug.WriteLine(oEx.Message);
            }
        }
    }
}
