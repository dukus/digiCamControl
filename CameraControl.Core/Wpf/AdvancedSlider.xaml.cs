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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#endregion

namespace CameraControl.Controls
{
    /// <summary>
    /// Interaction logic for AdvancedSlider.xaml
    /// </summary>
    public partial class AdvancedSlider : UserControl
    {
        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int Maximum
        {
            get { return (int) GetValue(MaximumProperty); }
            set
            {
                SetValue(MaximumProperty, value);
                if (Value > Maximum)
                    Value = Maximum;
            }
        }

        public int Minimum
        {
            get { return (int) GetValue(MinimumProperty); }
            set
            {
                SetValue(MinimumProperty, value);
                if (Value < Minimum)
                    Value = Minimum;
            }
        }


        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (int), typeof (AdvancedSlider),
                                        new FrameworkPropertyMetadata(100,
                                                                      FrameworkPropertyMetadataOptions.
                                                                          BindsTwoWayByDefault));

        private static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (AdvancedSlider),
                                        new FrameworkPropertyMetadata("Label",
                                                                      FrameworkPropertyMetadataOptions.
                                                                          BindsTwoWayByDefault));

        private static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof (int), typeof (AdvancedSlider),
                                        new FrameworkPropertyMetadata(0,
                                                                      FrameworkPropertyMetadataOptions.
                                                                          BindsTwoWayByDefault));

        private static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof (int), typeof (AdvancedSlider),
                                        new FrameworkPropertyMetadata(int.MaxValue,
                                                                      FrameworkPropertyMetadataOptions.
                                                                          BindsTwoWayByDefault));

        public AdvancedSlider()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btn_m_Click(object sender, RoutedEventArgs e)
        {
            Value--;
        }

        private void btn_p_Click(object sender, RoutedEventArgs e)
        {
            Value++;
        }
    }
}