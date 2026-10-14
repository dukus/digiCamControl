using System.Collections.Generic;

namespace CameraControl.Devices.Nikon
{
	internal class NikonD610 : NikonD600Base
	{
		protected override bool SupportsAEBracketing
			=> true;

		protected override IEnumerable<(long, string)> SupportedExposureEVSteps
		{
			get
			{
				yield return (0, "1/3 EV");
				yield return (1, "1/2 EV");
			}
		}

		protected override IEnumerable<(long, string)> SupportedAEBracketingSteps
		{
			get
			{
				yield return (0, "1/3 EV");
				yield return (1, "1/2 EV");
				yield return (2, "2/3 EV");
				yield return (3, "1 EV");
				yield return (4, "2 EV");
				yield return (5, "3 EV");
			}
		}

		protected override IEnumerable<(long code, int count, string range)> SupportedAEBracketingPatterns
		{
			get
			{
				yield return (0, 2, "-1..0");
				yield return (1, 2, "0..+1");
				yield return (2, 3, "-1..+1");
			}
		}
	}
}
