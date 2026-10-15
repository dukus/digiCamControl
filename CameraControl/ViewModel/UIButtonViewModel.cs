using GalaSoft.MvvmLight;

namespace CameraControl.ViewModel
{
	public sealed class UIButtonViewModel : ViewModelBase
	{
		public static UIButtonViewModel Instance
		{
			get;
		} = new UIButtonViewModel();
	}
}
