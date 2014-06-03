using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhotoBooth.Cards
{
    public class MirrorCardTemplate : PhotoCardTemplate<MirrorCard>
    {
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public MirrorCardTemplate()
            : base()
        {
            this.ImagesRequired = 1;
        }

        public override PhotoCard CreateCard()
        {
            MirrorCard card = new MirrorCard();
            card.DataContext = this;

            return card;
        }

        protected override UserControl CreateDesignerControl()
        {
            return null;
        }

        protected override void OnImageUpdated(int imageIndex, ImageSource imageSrc)
        {
            switch (imageIndex)
            {
                case 0:
                    this.Image = imageSrc;
                    break;
                default:
                    return;
            }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(MirrorCardTemplate), new PropertyMetadata(null));
    }
}
