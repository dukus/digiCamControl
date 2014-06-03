using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CameraControl.Controls
{
    /// <summary>
    /// Interaction logic for ExposureStatusControl.xaml
    /// </summary>
    public partial class ExposureStatusControl : UserControl
    {
        SolidColorBrush brushActive;
        SolidColorBrush brushInactive;
        Shape[] shapes;

        public ExposureStatusControl()
        {
            InitializeComponent();

            this.brushActive = this.Pos1.Fill as SolidColorBrush;

            this.brushInactive =
                new SolidColorBrush(
                Color.FromArgb(
                32,
                this.brushActive.Color.R,
                this.brushActive.Color.G,
                this.brushActive.Color.B));

            List<Shape> shapeList = new List<Shape>();

            shapeList.Add(this.PosMax); // 0
            shapeList.Add(this.Pos6);   // 1
            shapeList.Add(this.Pos5);   // 2
            shapeList.Add(this.Pos4);   // 3
            shapeList.Add(this.Pos3);   // 4
            shapeList.Add(this.Pos2);   // 5
            shapeList.Add(this.Pos1);   // 6
               
            shapeList.Add(this.Neg1);   // 7
            shapeList.Add(this.Neg2);   // 8
            shapeList.Add(this.Neg3);   // 9
            shapeList.Add(this.Neg4);   // 10
            shapeList.Add(this.Neg5);   // 11
            shapeList.Add(this.Neg6);   // 12        
            shapeList.Add(this.NegMax); // 13

            this.shapes = shapeList.ToArray();

            this.ExposureStatus = 0;
        }

        public static readonly DependencyProperty ExposureStatusProperty =
          DependencyProperty.Register("ExposureStatus", typeof(int), typeof(ExposureStatusControl),new PropertyMetadata(0,new PropertyChangedCallback(OnExposureStatusChanged)));

      private static void OnExposureStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
        ExposureStatusControl control = d as ExposureStatusControl;
        control.ExposureStatus = (int) e.NewValue;
      }

      public int ExposureStatus
      {
        set
        {
          foreach (Shape shape in this.shapes)
          {
            shape.Fill = this.brushInactive;
          }
          SetValue(ExposureStatusProperty, value);
          double absValue = Math.Abs(value);
          double delta =2.0; // For the D90, the granularity is 1/3 stop

          int count = (int) Math.Round(absValue/delta);
          count = Math.Min(count, 7);

          if (value > 0.0)
          {
            for (int i = 6; i > 6 - count; i--)
            {
              this.shapes[i].Fill = this.brushActive;
            }
          }
          else
          {
            for (int i = 7; i < 7 + count; i++)
            {
              this.shapes[i].Fill = this.brushActive;
            }
          }
        }
        get { return (int) GetValue(ExposureStatusProperty); }
      }


    }
}
