using System.Net.NetworkInformation;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class ExternalData:BaseFieldClass
    {
        private string _fileName;
        public string Row1 { get; set; }
        public string Row2 { get; set; }
        public string Row3 { get; set; }
        public string Row4 { get; set; }
        public string Row5 { get; set; }
        public string Row6 { get; set; }
        public string Row7 { get; set; }
        public string Row8 { get; set; }
        public string Row9 { get; set; }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }

        public string GetAllData()
        {
            return (Row1 + Row2 + Row3 + Row4 + Row5 + Row6 + Row7 + Row8 + Row9).ToLower();
        }
    }
}
