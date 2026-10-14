using System.Collections.Generic;

namespace CameraControl.Devices.Nikon
{
	public class NikonZ9 : NikonZ7_2
	{
		protected override IEnumerable<(long, string)> SupportedAEBracketingSteps
		{
			get
			{
				yield return (2, "1/3 EV");
				yield return (3, "1/2 EV");
				yield return (4, "2/3 EV");
				yield return (6, "1 EV");
				yield return (8, "1+1/3 EV");
				yield return (9, "1+1/2 EV");
				yield return (10, "1+2/3 EV");
				yield return (12, "2 EV");
				yield return (14, "2+1/3 EV");
				yield return (15, "2+1/2 EV");
				yield return (16, "2+2/3 EV");
				yield return (18, "3 EV");
			}
		}

		protected override IEnumerable<(long code, int count, string range)> SupportedAEBracketingPatterns
		{
			get
			{
				yield return (0, 0, "-");
				yield return (0x403, 3, "-1..+1");
				yield return (0x405, 5, "-2..+2");
				yield return (0x407, 7, "-3..+3");
				yield return (0x409, 9, "-4..+4");
			}
		}
	}
}
