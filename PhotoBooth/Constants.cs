
namespace PhotoBooth
{
    public static class Constants
    {
        public static int ScreenDPI
        { 
            get { return 96;}
        }

        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MBUTTONDOWN = 0x0207;

        public const byte VK_LSHIFT = 0xA0;
        public const int KEYEVENTF_KEYUP = 0x0002;
    }
}
