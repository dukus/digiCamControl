using CameraControl.Core;

namespace CameraControl.windows
{
    /// <summary>
    /// Interaction logic for PreviewWnd.xaml
    /// </summary>
    public partial class PreviewWnd 
    {
        public PreviewWnd()
        {
            InitializeComponent();
            ServiceProvider.Settings.ApplyTheme(this);
        }
    }
}
