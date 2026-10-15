using System.Collections.Generic;

namespace CameraControl.Devices.Nikon
{
	public class NikonZfc : NikonZ7_2
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
				yield return (2, "1/3 EV");
				yield return (4, "2/3 EV");
				yield return (6, "1 EV");
				yield return (12, "2 EV");
				yield return (18, "3 EV");
			}
		}
	}
}
