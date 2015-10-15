namespace CameraControl.Core.Classes
{
    public class PrintSettings
    {
        public string PrinterName { get; set; }
        public string PaperName { get; set; }
        public int PageWidth { get; set; }
        public int PageHeight { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int MarginBetweenImages { get; set; }
        public bool Repeat { get; set; }
        public bool Rotate { get; set; }
        public bool Fill { get; set; }

        public PrintSettings()
        {
            Rows = 1;
            Cols = 1;
        }
    }
}
