namespace CameraControl.Devices.Classes
{
    public class LiveViewData
    {
        public int LiveViewImageWidth { get; set; }
        public int LiveViewImageHeight { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public int FocusX { get; set; }
        public int FocusY { get; set; }

        public int FocusFrameXSize { get; set; }
        public int FocusFrameYSize { get; set; }


        public bool HaveFocusData { get; set; }

        public byte[] ImageData { get; set; }

        public bool Focused { get; set; }

        public int ImageDataPosition { get; set; }

        public int Rotation { get; set; }

        public bool MovieIsRecording { get; set; }
    }
}
