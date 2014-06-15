using System.Text;

namespace CameraControl.Devices.TransferProtocol.DDServer
{
    public class DdServerDevice
    {
        public DdServerDevice(byte[] data, int offset)
        {
            int index = offset;
            VendorId = (data[index] | (data[index + 1] << 8));
            index += 2;
            ProductId = (data[index] | (data[index + 1] << 8));
            index += 2;
            string[] s = Encoding.ASCII.GetString(data, index, data.Length - index).Split('\0');
            if (s.Length > 1)
            {
                VendorName = s[0];
                ProductName = s[1];
            }
        }

        public int VendorId { get; set; }
        public int ProductId { get; set; }

        public string VendorName { get; set; }
        public string ProductName { get; set; }
    }
}