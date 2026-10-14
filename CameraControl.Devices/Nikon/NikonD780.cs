using System.Collections.Generic;

namespace CameraControl.Devices.Nikon
{
	public class NikonD780 : NikonD750
	{
		protected override IEnumerable<(long, string)> SupportedExposureEVSteps
		{
			get
			{
				yield return (2, "1/3 EV");
				yield return (3, "1/2 EV");
				yield return (6, "1 EV");
			}
		}

		protected override IEnumerable<(long, string)> SupportedAEBracketingSteps
		{
			get
			{
				yield return (2, "1/3 EV");
				yield return (3, "1/2 EV");
				yield return (4, "2/3 EV");
				yield return (6, "1 EV");
				yield return (12, "2 EV");
				yield return (18, "3 EV");
			}
		}

		protected override IEnumerable<(long code, int count, string range)> SupportedAEBracketingPatterns
		{
			get
			{
				yield return (0, 0, "-");
				yield return (0x102, 2, "-1..0");
				yield return (0x202, 2, "0..+1");
				yield return (0x103, 3, "-2..0");
				yield return (0x203, 3, "0..+2");
				yield return (0x403, 3, "-1..+1");
				yield return (0x405, 5, "-2..+2");
				yield return (0x407, 7, "-3..+3");
				yield return (0x409, 9, "-4..+4");
			}
		}
	}
}
