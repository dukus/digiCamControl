using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhotoBooth.Cards
{
    public class TwoByTwoCardTemplate : PhotoCardTemplate<TwoByTwoCard>
    {
        public string BannerText
        {
            get { return (string)GetValue(BannerTextProperty); }
            set { SetValue(BannerTextProperty, value); }
        }

        public string BottomVerticalText
        {
            get { return (string)GetValue(BottomVerticalTextProperty); }
            set { SetValue(BottomVerticalTextProperty, value); }
        }

        public string TopVerticalText
        {
            get { return (string)GetValue(TopVerticalTextProperty); }
            set { SetValue(TopVerticalTextProperty, value); }
        }

        public ImageSource TopLeftImage
        {
            get { return (ImageSource)GetValue(TopLeftImageProperty); }
            set { SetValue(TopLeftImageProperty, value); }
        }

        public ImageSource TopRightImage
        {
            get { return (ImageSource)GetValue(TopRightImageProperty); }
            set { SetValue(TopRightImageProperty, value); }
        }

        public ImageSource BottomLeftImage
        {
            get { return (ImageSource)GetValue(BottomLeftImageProperty); }
            set { SetValue(BottomLeftImageProperty, value); }
        }

        public ImageSource BottomRightImage
        {
            get { return (ImageSource)GetValue(BottomRightImageProperty); }
            set { SetValue(BottomRightImageProperty, value); }
        }

        public TwoByTwoCardTemplate()
            : base()
        {
            this.ImagesRequired = 4;
            this.BannerText = Properties.Settings.Default.CardBannerText;
            this.BottomVerticalText = Properties.Settings.Default.CardBottomVerticalText;
            this.TopVerticalText = Properties.Settings.Default.CardTopVerticalText;
        }

        public override PhotoCard CreateCard()
        {
            TwoByTwoCard card = new TwoByTwoCard();
            card.DataContext = this;

            return card;
        }

        protected override UserControl CreateDesignerControl()
        {
            return new TwoByTwoDesigner();
        }

        protected override void OnImageUpdated(int imageIndex, ImageSource imageSrc)
        {
            switch (imageIndex)
            {
                case 0:
                    this.TopLeftImage = imageSrc;
                    break;
                case 1:
                    this.TopRightImage = imageSrc;
                    break;
                case 2:
                    this.BottomLeftImage = imageSrc;
                    break;
                case 3:
                    this.BottomRightImage = imageSrc;
                    break;
                default:
                    return;
            }
        }

        // Using a DependencyProperty as the backing store for BannerText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BannerTextProperty =
            DependencyProperty.Register("BannerText", typeof(string), typeof(TwoByTwoCardTemplate), new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for TopVerticalText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopVerticalTextProperty =
            DependencyProperty.Register("TopVerticalText", typeof(string), typeof(TwoByTwoCardTemplate), new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for BottomVerticalText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomVerticalTextProperty =
            DependencyProperty.Register("BottomVerticalText", typeof(string), typeof(TwoByTwoCardTemplate), new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for TopRightImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopRightImageProperty =
            DependencyProperty.Register("TopRightImage", typeof(ImageSource), typeof(TwoByTwoCardTemplate), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for TopLeftImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopLeftImageProperty =
            DependencyProperty.Register("TopLeftImage", typeof(ImageSource), typeof(TwoByTwoCardTemplate), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for BottomRightImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomRightImageProperty =
            DependencyProperty.Register("BottomRightImage", typeof(ImageSource), typeof(TwoByTwoCardTemplate), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for BottomLeftImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomLeftImageProperty =
            DependencyProperty.Register("BottomLeftImage", typeof(ImageSource), typeof(TwoByTwoCardTemplate), new PropertyMetadata(null));
    }
}
