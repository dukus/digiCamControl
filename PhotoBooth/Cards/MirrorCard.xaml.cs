using System.Windows;

namespace PhotoBooth.Cards
{
    /// <summary>
    /// Interaction logic for MirrorCard.xaml
    /// </summary>
    public partial class MirrorCard : PhotoCard
    {
        public MirrorCard()
        {
            InitializeComponent();
        }

        public override UIElement RootVisual
        {
            get { return this.LayoutGrid; }
        }
    }
}
