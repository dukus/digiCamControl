using System.Windows.Media;

namespace PhotoBooth
{
    public class PhotoCardInformation
    {
        public string BannerText { get; set; }
        public string BottomVerticalText { get; set; }
        public string TopVerticalText { get; set; }
        public ImageSource TopLeftImage { get; set; }
        public ImageSource TopRightImage { get; set; }
        public ImageSource BottomLeftImage { get; set; }
        public ImageSource BottomRightImage { get; set; }
    }
}
