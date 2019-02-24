using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibMJPEGServer;
using LibMJPEGServer.Sources;

namespace CameraControl.ViewModel
{
    class LiveViewVideoSource: VideoSource
    {
        public LiveViewVideoSource(LiveViewViewModel model)
        {
            
        }
        public override void StartCapture()
        {

        }

        public override void StopCapture()
        {

        }

        public override event EventHandler<FrameCapturedEventArgs> FrameCaptured;

        public void OnImageGrabbed(Image image)
        {
            

            FrameCaptured?.Invoke(this, new FrameCapturedEventArgs(image));
        }
    }
}
