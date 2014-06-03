using System.Windows;

namespace PhotoBooth.Cards
{
    /// <summary>
    /// Interaction logic for CardView.xaml
    /// </summary>
    public partial class TwoByTwoCard : PhotoCard
    {
        public TwoByTwoCard()
        {
            InitializeComponent();
        }

        public override UIElement RootVisual
        {
            get { return this.LayoutGrid; }
        }
    }
}
