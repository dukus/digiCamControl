using System.Collections.Generic;

namespace CameraControl.Devices.Nikon
{
	public class NikonD5500 : NikonD5200
	{
		protected override IEnumerable<(long code, int count, string range)> SupportedAEBracketingPatterns
		{
			get
			{
				yield return (4, 3, "-1..+1");
			}
		}
	}
}
