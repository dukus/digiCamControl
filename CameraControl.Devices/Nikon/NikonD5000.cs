namespace CameraControl.Devices.Nikon
{
	public class NikonD5000 : NikonD90
	{
		protected override bool SupportsAEBracketing
			// The AE bracketing patterns are not documented
			=> false;
	}
}
