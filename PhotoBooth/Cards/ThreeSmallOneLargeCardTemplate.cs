using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhotoBooth.Cards
{
    public class ThreeSmallOneLargeCardTemplate : PhotoCardTemplate<ThreeSmallOneLargeCard>
    {
        public string BannerText
        {
            get { return (string)GetValue(BannerTextProperty); }
            set { SetValue(BannerTextProperty, value); }
        }

        public ThreeSmallOneLargeCardTemplate()
            : base()
        {
            this.ImagesRequired = 4;
            this.BannerText = Properties.Settings.Default.CardBannerText;
        }

        public override PhotoCard CreateCard()
        {
            ThreeSmallOneLargeCard card = new ThreeSmallOneLargeCard();
            card.DataContext = this;

            return card;
        }

        protected override UserControl CreateDesignerControl()
        {
            return new ThreeSmallOneLargeDesigner();
        }

        protected override void OnImageUpdated(int imageIndex, ImageSource imageSrc)
        {
            switch (imageIndex)
            {
                case 0:
                    this.TopLeftImage = imageSrc;
                    break;
                case 1:
                    this.TopMiddleImage = imageSrc;
                    break;
                case 2:
                    this.TopRightImage = imageSrc;
                    break;
                case 3:
                    this.BottomImage = imageSrc;
                    break;
                default:
                    return;
            }
        }

        public ImageSource TopLeftImage
        {
            get { return (ImageSource)GetValue(TopLeftImageProperty); }
            set { SetValue(TopLeftImageProperty, value); }
        }

        public ImageSource TopMiddleImage
        {
            get { return (ImageSource)GetValue(TopMiddleImageProperty); }
            set { SetValue(TopMiddleImageProperty, value); }
        }

        public ImageSource TopRightImage
        {
            get { return (ImageSource)GetValue(TopRightImageProperty); }
            set { SetValue(TopRightImageProperty, value); }
        }

        public ImageSource BottomImage
        {
            get { return (ImageSource)GetValue(BottomImageProperty); }
            set { SetValue(BottomImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BannerText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BannerTextProperty =
            DependencyProperty.Register("BannerText", typeof(string), typeof(ThreeSmallOneLargeCardTemplate), new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for BottomImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomImageProperty =
            DependencyProperty.Register("BottomImage", typeof(ImageSource), typeof(ThreeSmallOneLargeCardTemplate), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for TopRightImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopRightImageProperty =
            DependencyProperty.Register("TopRightImage", typeof(ImageSource), typeof(ThreeSmallOneLargeCardTemplate), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for TopMiddleImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopMiddleImageProperty =
            DependencyProperty.Register("TopMiddleImage", typeof(ImageSource), typeof(ThreeSmallOneLargeCardTemplate), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for TopLeftImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopLeftImageProperty =
            DependencyProperty.Register("TopLeftImage", typeof(ImageSource), typeof(ThreeSmallOneLargeCardTemplate), new PropertyMetadata(null));
    }
}
