using System.Collections.Generic;

namespace CameraControl.Devices.Nikon
{
	public class NikonDf : NikonD500Base
	{
		protected override IEnumerable<(long, string)> SupportedExposureEVSteps
		{
			get
			{
				yield break;
			}
		}

		protected override IEnumerable<(long, string)> SupportedAEBracketingSteps
		{
			get
			{
				yield return (0, "1/3 EV");
				yield return (1, "-");
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
				yield return (2, 3, "-2..0");
				yield return (3, 3, "0..+2");
				yield return (4, 3, "-1..+1");
				yield return (5, 5, "-2..+2");
			}
		}
	}
}
