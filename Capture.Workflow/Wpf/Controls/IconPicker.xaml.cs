using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace Capture.Workflow.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for IconPicker.xaml
    /// </summary>
    public partial class IconPicker : UserControl
    {

        #region Dependency properties
        public string SelectedIcon
        {
            get { return (string)GetValue(SelectedIconProperty); }
            set { SetValue(SelectedIconProperty, value); }
        }

        public static readonly DependencyProperty SelectedIconProperty =
            DependencyProperty.Register("SelectedIcon", typeof(string), typeof(IconPicker),
                new FrameworkPropertyMetadata(OnSelectedColorChanged));

        private static void OnSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {


        }

        #endregion
        public IconPicker()
        {
            InitializeComponent();
            AddIcons();
        }

        private void AddIcons()
        {
            var packIconKinds =
                Enum.GetNames(typeof(PackIconKind))
                    .OrderBy(k => k, StringComparer.InvariantCultureIgnoreCase)
                    .ToList();
            IconList.Items.Add("(None)");
            foreach (var kind in packIconKinds)
            {
                IconList.Items.Add(kind);
            }
           
        }
    }
}
