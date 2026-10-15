using System.IO.Ports;
using System.Linq;

namespace CameraControl.Core.Classes
{
	public class ScriptExternalObjectCOMPortParameter : ScriptExternalObjectCreateParameter
	{
		public ScriptExternalObjectCOMPortParameter (string name = null)
			: base(name ?? "Serial (COM) port of the device", ValueType.String)
		{
			IsConstant = false;
		}

		public override void RefreshEnumerationValues ()
		{
			EnumerationValues = (from c in SerialPort.GetPortNames()
								 orderby (c.StartsWith("COM") ? int.Parse(c.Substring(3)) : int.MaxValue), c.ToLower()
								 select c).ToList();
		}
	}
}
