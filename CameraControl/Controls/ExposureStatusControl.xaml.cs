#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CameraControl.Devices;

#endregion

namespace CameraControl.Controls
{
    /// <summary>
    /// Interaction logic for ExposureStatusControl.xaml
    /// </summary>
    public partial class ExposureStatusControl : UserControl
    {
        private SolidColorBrush brushActive;
        private SolidColorBrush brushInactive;
        private Shape[] shapes;

        public ExposureStatusControl()
        {
            InitializeComponent();

            brushActive = Pos1.Fill as SolidColorBrush;

            brushInactive =
                new SolidColorBrush(
                    Color.FromArgb(
                        32,
                        brushActive.Color.R,
                        brushActive.Color.G,
                        brushActive.Color.B));

            List<Shape> shapeList = new List<Shape>();

            shapeList.Add(PosMax); // 0
            shapeList.Add(Pos6); // 1
            shapeList.Add(Pos5); // 2
            shapeList.Add(Pos4); // 3
            shapeList.Add(Pos3); // 4
            shapeList.Add(Pos2); // 5
            shapeList.Add(Pos1); // 6

            shapeList.Add(Neg1); // 7
            shapeList.Add(Neg2); // 8
            shapeList.Add(Neg3); // 9
            shapeList.Add(Neg4); // 10
            shapeList.Add(Neg5); // 11
            shapeList.Add(Neg6); // 12        
            shapeList.Add(NegMax); // 13

            shapes = shapeList.ToArray();

            ExposureStatus = 0;
        }

        public static readonly DependencyProperty ExposureStatusProperty =
            DependencyProperty.Register("ExposureStatus", typeof (int), typeof (ExposureStatusControl),
                                        new PropertyMetadata(0, new PropertyChangedCallback(OnExposureStatusChanged)));

        private static void OnExposureStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExposureStatusControl control = d as ExposureStatusControl;
            control.ExposureStatus = (int) e.NewValue;
        }

        public int ExposureStatus
        {
            set
            {
                try
                {
                    foreach (Shape shape in shapes)
                    {
                        shape.Fill = brushInactive;
                    }
                    SetValue(ExposureStatusProperty, value);
                    double absValue = Math.Abs(value);
                    double delta = 2.0; // For the D90, the granularity is 1/3 stop

                    int count = (int)Math.Round(absValue / delta);
                    count = Math.Min(count, 7);

                    if (value > 0.0)
                    {
                        for (int i = 6; i > 6 - count; i--)
                        {
                            shapes[i].Fill = brushActive;
                        }
                    }
                    else
                    {
                        for (int i = 7; i < 7 + count; i++)
                        {
                            shapes[i].Fill = brushActive;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("EXS error", ex);
                }
            }
            get { return (int) GetValue(ExposureStatusProperty); }
        }
    }
}