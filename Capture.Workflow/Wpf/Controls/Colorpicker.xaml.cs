using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CameraControl.Core.Wpf
{
    /// <summary>
    /// Colorpicker user control
    /// This code is open source published with the Code Project Open License (CPOL).
    ///
    /// Originally written by Øystein Bjørke, March 2009.
    /// 
    /// The code and accompanying article can be found at http://www.codeproject.com
    /// </summary>

    public partial class Colorpicker : UserControl
    {
        #region Dependency properties
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(Colorpicker),
            new FrameworkPropertyMetadata(OnSelectedColorChanged));

        private static void OnSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

            Colorpicker cp = obj as Colorpicker;
            Debug.Assert(cp != null);

            Color newColor = (Color)args.NewValue;
            Color oldColor = (Color)args.OldValue;

            if (newColor == oldColor)
                return;

            // When the SelectedColor changes, set the selected value of the combo box
            ColorViewModel selectedColorViewModel = cp.ColorList1.SelectedValue as ColorViewModel;
            if (selectedColorViewModel == null || selectedColorViewModel.Color != newColor)
            {
                // Add the color if not found
                if (!cp.ListContains(newColor))
                {
                    cp.AddColor(newColor, newColor.ToString());
                }
            }

            // Also update the brush
            cp.SelectedBrush = new SolidColorBrush(newColor);
            cp.OnColorChanged(oldColor, newColor);
        }

        private bool ListContains(Color newColor)
        {
            foreach (object o in ColorList1.Items)
            {
                ColorViewModel vcm = o as ColorViewModel;
                if (vcm == null) continue;
                if (vcm.Color == newColor) return true;
            }
            return false;
        }

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(Colorpicker),
            new FrameworkPropertyMetadata(OnSelectedBrushChanged));

        private static void OnSelectedBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // Debug.WriteLine("OnSelectedBrushChanged");
            Colorpicker cp = (Colorpicker)obj;
            SolidColorBrush newBrush = (SolidColorBrush)args.NewValue;
            // SolidColorBrush oldBrush = (SolidColorBrush)args.OldValue;

            if (cp.SelectedColor != newBrush.Color)
                cp.SelectedColor = newBrush.Color;
        }
        #endregion

        #region Events
        public static readonly RoutedEvent ColorChangedEvent =
            EventManager.RegisterRoutedEvent("ColorChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color>), typeof(Colorpicker));

        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        protected virtual void OnColorChanged(Color oldValue, Color newValue)
        {
            RoutedPropertyChangedEventArgs<Color> args = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue);
            args.RoutedEvent = Colorpicker.ColorChangedEvent;
            RaiseEvent(args);
        }
        #endregion

        static Brush _CheckerBrush = CreateCheckerBrush();
        public static Brush CheckerBrush { get { return _CheckerBrush; } }
        // Todo: should this be disposed somewhere?

        public Colorpicker()
        {
            InitializeComponent();

            InitializeColors();
        }

        public void InitializeColors()
        {
            ColorList1.Items.Clear();

            // Add some common colors
            AddColor(Colors.Black, "Black");
            AddColor(Colors.Gray, "Gray");
            AddColor(Colors.LightGray, "LightGray");
            AddColor(Colors.White, "White");
            AddColor(Colors.Transparent, "Transparent");
            AddColor(Colors.Red, "Red");
            AddColor(Colors.Green, "Green");
            AddColor(Colors.Blue, "Blue");
            AddColor(Colors.Cyan, "Cyan");
            AddColor(Colors.Magenta, "Magenta");
            AddColor(Colors.Yellow, "Yellow");
            AddColor(Colors.Purple, "Purple");
            AddColor(Colors.Orange, "Orange");
            AddColor(Colors.Brown, "Brown");

            // And some colors with transparency
            AddColor(Color.FromArgb(128, 0, 0, 0), "Black 50%");
            AddColor(Color.FromArgb(128, 255, 255, 255), "White 50%");
            AddColor(Color.FromArgb(128, 255, 0, 0), "Red 50%");
            AddColor(Color.FromArgb(128, 0, 255, 0), "Green 50%");
            AddColor(Color.FromArgb(128, 0, 0, 255), "Blue 50%");
            ColorList1.Items.Add(new Separator());

            // Enumerate constant colors from the Colors class
            Type colorsType = typeof(Colors);
            PropertyInfo[] pis = colorsType.GetProperties();
            foreach (PropertyInfo pi in pis)
                AddColor((Color)pi.GetValue(null, null), pi.Name);

            // todo: does this work?
            ColorList1.SelectedValuePath = "Color";
        }


        private void AddColor(Color color, string name)
        {
            if (!name.StartsWith("#", StringComparison.Ordinal))
                name = NiceName(name);
            ColorViewModel cvm = new ColorViewModel() { Color = color, Name = name };
            ColorList1.Items.Add(cvm);
        }

        private static string NiceName(string name)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]))
                    sb.Append(" ");
                sb.Append(name[i]);
            }
            return sb.ToString();
        }

        public static Brush CreateCheckerBrush()
        {
            // from http://msdn.microsoft.com/en-us/library/aa970904.aspx

            DrawingBrush checkerBrush = new DrawingBrush();

            GeometryDrawing backgroundSquare =
                new GeometryDrawing(
                    Brushes.White,
                    null,
                    new RectangleGeometry(new Rect(0, 0, 8, 8)));

            GeometryGroup aGeometryGroup = new GeometryGroup();
            aGeometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, 4, 4)));
            aGeometryGroup.Children.Add(new RectangleGeometry(new Rect(4, 4, 4, 4)));

            GeometryDrawing checkers = new GeometryDrawing(Brushes.Black, null, aGeometryGroup);

            DrawingGroup checkersDrawingGroup = new DrawingGroup();
            checkersDrawingGroup.Children.Add(backgroundSquare);
            checkersDrawingGroup.Children.Add(checkers);

            checkerBrush.Drawing = checkersDrawingGroup;
            checkerBrush.Viewport = new Rect(0, 0, 0.5, 0.5);
            checkerBrush.TileMode = TileMode.Tile;

            return checkerBrush;
        }

    }

    public class ColorViewModel
    {
        public Color Color { get; set; }
        public Brush Brush { get { return new SolidColorBrush(Color); } }
        public string Name { get; set; }
    }
}
