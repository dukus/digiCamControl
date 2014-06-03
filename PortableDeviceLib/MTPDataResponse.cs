using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableDeviceLib
{
  public class MTPDataResponse
  {
    public byte[] Data { get; set; }
    public uint ErrorCode { get; set; }
  }
}
