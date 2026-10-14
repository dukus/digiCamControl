using System.Collections.Generic;

namespace CameraControl.Devices.Nikon
{
	public class NikonD3 : NikonD90
	{
		protected override IEnumerable<(long, string)> SupportedAEBracketingSteps
		{
			get
			{
				yield return (0, "1/3 EV");
				yield return (1, "1/2 EV");
				yield return (2, "2/3 EV");
				yield return (3, "1 EV");
			}
		}

		protected override IEnumerable<(long code, int count, string range)> SupportedAEBracketingPatterns
		{
			get
			{
				yield return (0, 2, "-1..0");
				yield return (1, 2, "0..+1");
				yield return (2, 3, "-2..0");
				yield return (3, 3, "0..+2");
				yield return (4, 3, "-1..+1");
				yield return (5, 5, "-2..+2");
				yield return (6, 7, "-3..+3");
				yield return (7, 9, "-4..+4");
			}
		}
	}
}
