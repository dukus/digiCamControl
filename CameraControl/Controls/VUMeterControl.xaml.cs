using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CameraControl.Controls
{
  /// <summary>
  /// Interaction logic for VUMeterControl.xaml
  /// </summary>
  public partial class VUMeterControl : UserControl
  {
    #region Inner Class
    public class Block
    {
      public int Level { get; set; } // 1: Red, 2: Yellow, 3: Green
    }
    #endregion

    private ObservableCollection<Block> items = new ObservableCollection<Block>();
    public VUMeterControl()
    {
      InitializeComponent();
      GenerateBlocks();
      this.PART_ItemsPresenter.DataContext = items;
    }

    private void GenerateBlocks()
    {
      items.Clear();
      for (int i = 0; i < BlockCount; i++)
      {
        if (i < BlockCount - HighLevel) items.Add(new Block() { Level = 1 });
        if (i >= BlockCount - HighLevel && i < BlockCount - MiddleLevel) items.Add(new Block() { Level = 2 });
        if (i >= BlockCount - MiddleLevel) items.Add(new Block() { Level = 3 });
      }
    }

    #region Dependency Properties

    public int BlockCount
    {
      get { return (int)GetValue(BlockCountProperty); }
      set { SetValue(BlockCountProperty, value); }
    }
    public static readonly DependencyProperty BlockCountProperty =
        DependencyProperty.Register("BlockCount", typeof(int), typeof(VUMeterControl), new UIPropertyMetadata(15, (o, e) =>
        {
          ((VUMeterControl)o).GenerateBlocks();
        }));

    public int HighLevel
    {
      get { return (int)GetValue(HighLevelProperty); }
      set { SetValue(HighLevelProperty, value); }
    }
    public static readonly DependencyProperty HighLevelProperty =
        DependencyProperty.Register("HighLevel", typeof(int), typeof(VUMeterControl), new UIPropertyMetadata(12, (o, e) =>
        {
          ((VUMeterControl)o).GenerateBlocks();
        }));

    public int MiddleLevel
    {
      get { return (int)GetValue(MiddleLevelProperty); }
      set { SetValue(MiddleLevelProperty, value); }
    }
    public static readonly DependencyProperty MiddleLevelProperty =
        DependencyProperty.Register("MiddleLevel", typeof(int), typeof(VUMeterControl), new UIPropertyMetadata(7, (o, e) =>
        {
          ((VUMeterControl)o).GenerateBlocks();
        }));

    public int MaxValue
    {
      get { return (int)GetValue(MaxValueProperty); }
      set { SetValue(MaxValueProperty, value); }
    }
    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(int), typeof(VUMeterControl), new UIPropertyMetadata(100));

    public int Value
    {
      get { return (int)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(int), typeof(VUMeterControl), new UIPropertyMetadata(0,
          (sender, e) =>
          {
            VUMeterControl control = (VUMeterControl)sender;
            int value = (int)e.NewValue;
              for (int i = control.BlockCount - 1; i >= 0; i--)
              {
                  FrameworkElement element =
                      (FrameworkElement) control.PART_ItemsPresenter.ItemContainerGenerator.ContainerFromIndex(i);
                  if (element != null)
                  {
                      if (control.BlockCount - i > value/(control.MaxValue/control.BlockCount))
                          element.Visibility = Visibility.Hidden;
                      else
                          element.Visibility = Visibility.Visible;
                  }
              }
          })
        );

    public int PeakValue
    {
        get { return (int)GetValue(PeakValueProperty); }
        set { SetValue(PeakValueProperty, value); }
    }

      public static readonly DependencyProperty PeakValueProperty =
          DependencyProperty.Register("PeakValue", typeof (int), typeof (VUMeterControl), new UIPropertyMetadata(0,
              (sender, e) =>
              {
                  VUMeterControl control = (VUMeterControl) sender;
                  int value = (int) e.NewValue;
                  int i = value/(control.MaxValue/control.BlockCount);
                  FrameworkElement element =
                      (FrameworkElement) control.PART_ItemsPresenter.ItemContainerGenerator.ContainerFromIndex(i);
                  if (element != null)
                  {
                      element.Visibility = Visibility.Visible;
                  }
              })
              );

      #endregion

  }
}
