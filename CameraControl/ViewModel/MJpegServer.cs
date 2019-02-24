using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Kernels;
using LibMJPEGServer;
using LibMJPEGServer.QualityManagement;
using LibMJPEGServer.Sources;
using LibThreadedSockets;
using Log = CameraControl.Devices.Log;

namespace CameraControl.ViewModel
{
    public class MJpegServer
    {
        private const string ServerUrl = "/live";
        private readonly ThreadedServer _server;
        private readonly MJPEGStreamer _streamer;

        public string ServerAddress
        {
            get
            {
                return this._server.GetServerAddress() + "/live";
            }
        }

        public event EventHandler<StreamErrorEventArgs> StreamError;

        public MJpegServer(int port, VideoSource source, QualityDefinition qualityDefinition)
        {
            this._server = new ThreadedServer(port, System.Net.IPAddress.Parse("127.0.0.1") , 100);
            this._server.ClientDataReceived += new ThreadedServer.ClientDataReceivedEventHandler(this.OnClientDataReceived);
            this._server.SocketError += new ThreadedServer.SocketErrorEventHandler(this.OnSocketError);
            this._streamer = new MJPEGStreamer(source, qualityDefinition);
        }

        public void Start()
        {
            try
            {
                this._server.Start();
            }
            catch (Exception e)
            {
            }
        }

        public void Stop()
        {
            try
            {
                this._server.Stop();
            }
            catch (Exception e)
            {
                
            }
        }

        private void OnClientDataReceived(object sender, ClientDataReceivedEventArgs e)
        {
            if (!Encoding.UTF8.GetString(e.Data).Contains(string.Format("GET {0}", (object)"/live")))
                this._server.DisconnectClient(e.ClientConnection);
            else
                this._streamer.AddClient(e.ClientConnection);
        }

        private void OnSocketError(object sender, SocketErrorEventArgs e)
        {
            Log.Error("MJpeg server error "+ e.Message);
            // ISSUE: reference to a compiler-generated field
            EventHandler<StreamErrorEventArgs> streamError = this.StreamError;
            if (streamError == null)
                return;
            streamError((object)this, new StreamErrorEventArgs(string.Format("Socket error: {0}", (object)e.Message), e));

        }
    }
}
