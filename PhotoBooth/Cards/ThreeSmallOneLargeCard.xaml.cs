using System.Windows;

namespace PhotoBooth.Cards
{
    /// <summary>
    /// Interaction logic for ThreeByOneCard.xaml
    /// </summary>
    public partial class ThreeSmallOneLargeCard : PhotoCard
    {
        public ThreeSmallOneLargeCard()
        {
            InitializeComponent();
        }

        public override UIElement RootVisual
        {
            get { return this.LayoutGrid; }
        }
    }
}
